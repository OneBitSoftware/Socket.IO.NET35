var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http, {
    pingInterval: 5000,
    //transports: ["websocket"],
    //upgrade: false
});
var expect = require('expect.js');
var test_data = require('./test_data.json');

app.get('/', function (req, res) {
    res.sendFile(__dirname + '/index.html');
});

io.on('connection', function (socket) {
    console.log('connected... ' + socket.id);

    socket.on('hi', function (msg) {
        console.log('Received hi emit data: ' + msg);
        io.sockets.emit('hi', msg);
    });

    socket.on('disconnect', function (socket) {
        console.log('Got disconnected...' + socket);
    });

    socket.on('connect', function (socket) { // does nothing
        console.log('socket connected:' + socket);
    });

    socket.on('reconnect', function (socket) {
        console.log('socket reconnected:' + socket);
    });

    //ogs test
    socket.on('parser_error#21', function (d) {
        console.log("ogs test" + d);
        socket.emit('parser_error#21_response', test_data.ogstestchars);
    });

    socket.on('d10000chars', function () {
        console.log('d10000chars');
        socket.emit('d10000chars', test_data.d10000chars);
    });


    socket.on('d100000chars', function () {
        console.log('d100000chars');
        socket.emit('d100000chars', test_data.d100000chars);
    });


    socket.on('json10000chars', function () {
        console.log('json10000chars');
        socket.emit('json10000chars', { data: test_data.d10000chars });
    });

    socket.on('json10000000chars', function () {
        console.log('json10000000chars');
        socket.emit('json10000000chars', {
            data: test_data.d10000000chars,
            data2: test_data.d100000chars,
            data3: test_data.d100000chars,
            data4: { data5: test_data.d100000chars }
        });
    });


    socket.on('latin', function (wsinput) {
        console.log('issue24 socket.on latin');
        socket.emit('latin', { 'error': 'Nombre de usuario o contraseña incorrecta.' });
    });

    socket.on('nolatin', function (wsinput) {
        console.log('issue24 sockect.on no latin');
        socket.emit('nolatin', { 'error': 'Nombre de usuario o contrasena incorrecta.' });
    });

    socket.on('get_cookie', function () {
        console.log(util.inspect(socket.handshake.headers.cookie));
        socket.emit('got_cookie', socket.handshake.headers.cookie);
    });

    // ack tests
    socket.on('ack', function () {
        socket.emit('ack', function (a, b) {
            console.log("emit ack b=" + JSON.stringify(b));
            if (a === 5 && b.b === true) {
                socket.emit('got it');
            }
        });
    });

    socket.on('ack2', function () {
        socket.emit('ack2', 'hello there', function (a, b) {
            console.log("emit ack2 b=" + JSON.stringify(b));
            if (a === 5 && b.b === true) {
                socket.emit('got it');
            }
        });
    });

    socket.on('getAckDate', function (data, cb) {
        cb(new Date(), 5);
    });

    socket.on('getDate', function () {
        socket.emit('takeDate', new Date());
    });

    socket.on('getDateObj', function () {
        socket.emit('takeDateObj', { date: new Date() });
    });

    socket.on('getUtf8', function () {
        socket.emit('takeUtf8', 'てすと');
        socket.emit('takeUtf8', 'Я Б Г Д Ж Й');
        socket.emit('takeUtf8', 'Ä ä Ü ü ß');
        socket.emit('takeUtf8', '李O四');
        socket.emit('takeUtf8', 'utf8 — string');
    });

    // false test
    socket.on('false', function () {
        socket.emit('false', false);
    });

    // binary test
    socket.on('doge', function () {
        var buf = new Buffer('asdfasdf', 'utf8');
        socket.emit('doge', buf);
    });

    // expect receiving binary to be buffer
    socket.on('buffa', function (a) {
        if (Buffer.isBuffer(a)) {
            socket.emit('buffack');
        }
    });

    // expect receiving binary with mixed JSON
    socket.on('jsonbuff', function (a) {
        expect(a.hello).to.eql('lol');
        expect(Buffer.isBuffer(a.message)).to.be(true);
        expect(a.goodbye).to.eql('gotcha');
        socket.emit('jsonbuff-ack');
    });

    // expect receiving buffers in order
    var receivedAbuff1 = false;
    socket.on('abuff1', function (a) {
        expect(Buffer.isBuffer(a)).to.be(true);
        receivedAbuff1 = true;
    });
    socket.on('abuff2', function (a) {
        expect(receivedAbuff1).to.be(true);
        socket.emit('abuff2-ack');
    });

    // emit buffer to base64 receiving browsers
    socket.on('getbin', function () {
        var buf = new Buffer('asdfasdf', 'utf8');
        socket.emit('takebin', buf);
    });

    // simple test
    socket.on('test', function (d) {
        var s1 = "test" + d;
        console.log(s1);
        fs.appendFileSync('test.txt', s1);
        socket.emit('hi', 'more data');
    });

});

io.of('/timeout_socket').on('connection', function () {
    // register namespace
});

io.of('/foo').on('connection', function () {
    // register namespace
});

io.of('/timeout_socket').on('connection', function () {
    // register namespace
});

io.of('/valid').on('connection', function () {
    // register namespace
});

io.of('/asd').on('connection', function () {
    // register namespace
});

http.listen(3000, function () {
    console.log('listening on *:3000');
});
