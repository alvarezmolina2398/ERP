$(function () {
    $('#FrmLogin').submit(function () {
        var user = $('#user').val();
        var pass = $('#pass').val();

        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: 'vista/wsvalidar.asmx/verificacion',
            data: '{cadena: "' + base64(user + '|' + pass) + '"}',
            dataType: "json",
            success: function (data) {
                if (data.d != "e") {
                    location.href = '/ERP2/vista/principal.html';
                    document.cookie = "us=" + base64(user);
                }
                else {
                    $('#user').val("");
                    $('#pass').val("");
                    alert('usuario o contrasena incorrectos');
                }
            }
        });
        return false;
    });

    function base64(cadena) {
        return window.btoa(unescape(encodeURIComponent(cadena)));
    }
});