$(function () {

    $(document).ready(function () {
        var cookie = getCookie("us");
        if (cookie != '') {
            listaPermisos();
        }
        else {
            alert("vuelve a iniciar sesion");
            location.href = '/Login.html';
        }
    });


    $('#btnCerrar').click(function () {
        eliminarCookie("us");
        location.href = '/Login.html';
    });



    var eliminarCookie = function (key) {
        return document.cookie = key + '=;expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    }

    function listaPermisos() {
        var usr = window.atob(getCookie("us"));


        $.ajax({
            type: "POST",
            url: "wspermisos.asmx/obtenerPermisos",
            data: '{us: "' + usr + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (data) {
            },
            success: function (msg) {
                var html = "";
                var men = "";
                var dir = ""
                var cambio = 0;
                $.each(msg.d, function () {

                    if (men != this.menu && cambio == 1) {
                        html += '</ul>';
                        html += '</li>';
                        cambio = 0;
                        men = this.menu;
                    }

                    if (cambio == 0) {
                        html += '<li class="nav-item">';
                        html += '<a href="javascript:void(0);"  class="' + this.id + ' nav-link dropdwown-toggle" onclick="mostrarm(' + this.id + ')" ><i class="material-icons icon">work</i> <span>' + this.menu + '</span><i class="material-icons icon arrow">expand_more</i></a>';
                        html += '<ul class="nav flex-column">';
                        cambio = 1;
                        men = this.menu;
                    }

                    if (cambio == 1) {
                        html += '<li class="nav-item">';
                        html += '<a href="' + this.direccion + '" class="nav-link blue-darken-active"><i class="material-icons icon"></i> <span>' + this.descripcion + '</span></a>';
                        html += '</li>';
                    }


                });
                $('#allMenu').append(html);

            }
        });
    }

    function getCookie(cname) {
        var name = cname + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }




});

/* left sidebar accordion menu */
/* url  navigation active */
var url = window.location;

function menuitems() {
    var element = $('.sidebar .nav .nav-item a').filter(function () {
        return this.href == url;
        console.log(url)
    }).addClass('active').parent("li").addClass('active').closest('.nav').slideDown().addClass('in').prev().addClass('active').parent().addClass('show').closest('.nav').slideDown().addClass('in').parent().addClass('show');
}
menuitems();

function mostrarm(id) {
    if ($('.' + id).hasClass('active') != true) {
        $('.sidebar .nav .nav-item .dropdwown-toggle').removeClass('active').next().slideUp();
        $('.' + id).addClass('active').next().slideDown();
    } else {
        $('.' + id).removeClass('active').next().slideUp();
    }
};

