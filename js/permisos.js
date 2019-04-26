$(function () {
    $(document).ready(function () {
        listarol();
        listaMenu();
    });

    //metodo utilizado para mostrar el listado de roles
    function listarol() {
        $.ajax({
            type: "POST",
            url: "wsroles.asmx/ObtenerDatos",
            data: '',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (data) {
            },
            success: function (msg) {
                $.each(msg.d, function () {

                    $('#rol').append('<option value="' + this.id + '">' + this.nombre + '</option>');

                });
            }
        });
    }

    //metodo utilizado para mostrar el listado de roles
    function listaMenu() {
        $.ajax({
            type: "POST",
            url: "wspermisos.asmx/obtenerMenu",
            data: '',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (data) {
            },
            success: function (msg) {
                $.each(msg.d, function () {

                    $('#menui').append('<option value="' + this.id + '">' + this.descripcion + '</option>');

                });
            }
        });
    }

    function cargarPrivilegiosAsignados(rol, menu) {
        var i = 0;
        $.ajax({
            type: "POST",
            url: "wspermisos.asmx/obtenerAsignados",
            contentType: "application/json; charset=utf-8",
            data: '{menu: ' + menu + ',rol: ' + rol + '}',
            dataType: "json",
            success: function (msg) {
                $('#Pnasignadas').html(null);
                $.each(msg.d, function () {
                    $('#Pnasignadas').append('<div class="col-md-12 checkbox"><input value="' + this.id + '" type="checkbox" name="asignados" id="asig' + i + '"><label for="asig' + i + '"> ' + this.descripcion + ' </label></div> ');
                    i++;
                });
            },
            error: function (msg) {
                // mensajeerrorpermanente(msg.responseText);
            }
        });
    }


    function cargarPrivilegiosNoAsignados(rol, menu) {
        var i = 0;
        $.ajax({
            type: "POST",
            url: "wspermisos.asmx/obtenerNoAsignados",
            contentType: "application/json; charset=utf-8",
            data: '{menu: ' + menu + ',rol: ' + rol + '}',
            dataType: "json",
            success: function (msg) {
                $('#Pnnoasignadas').html(null);
                $.each(msg.d, function () {
                    $('#Pnnoasignadas').append('<div class="col-md-12 checkbox"><input value="' + this.id + '" type="checkbox" name="noasignados" id="noasig' + i + '"><label for="noasig' + i + '"> ' + this.descripcion + ' </label></div> ');
                    i++;
                });
            },
            error: function (msg) {
                // mensajeerrorpermanente(msg.responseText);
            }
        });
    }


    $('#btn-Agregar').click(function () {
        var menu = $('#menui').val();
        var rol = $('#rol').val();
        var agregar = [];
        $("input[name='noasignados']").each(function () {
            if ($(this).prop('checked')) {
                agregar.push(parseInt($(this).val()))
            }
        });

        var jsonArray = JSON.stringify(agregar);
        $.ajax({
            type: "POST",
            url: "wspermisos.asmx/addPermisos",
            contentType: "application/json; charset=utf-8",
            data: "{permiso: " + jsonArray + ", rol:" + rol + ", menu : " + menu + " }",
            success: function (msg) {
                llenar();
                $.toast({
                    heading: 'Listo!',
                    text: 'Permisos asignados exitosamente',
                    showHideTransition: 'slide',
                    position: 'mid-center',
                    icon: 'success'
                })
            }
        });

    });

    $('#btn-Quitar').click(function () {
        var rol = $('#rol').val();
        var quitar = "";
        $("input[name='asignados']").each(function () {
            if ($(this).prop('checked')) {
                if (quitar == "") {
                    quitar = $(this).val()
                } else {
                    quitar = (quitar + "," + $(this).val()).toString()
                }

            }
        });

        $.ajax({
            type: "POST",
            url: "wspermisos.asmx/delPermisos",
            contentType: "application/json; charset=utf-8",
            data: "{permiso: '" + quitar + "', rol: " + rol + "}",
            success: function (msg) {

                $.toast({
                    heading: 'Listo!',
                    text: 'los permisos fueron eliminados exitosamente',
                    showHideTransition: 'slide',
                    position: 'mid-center',
                    icon: 'success'
                })
                llenar();
            }
        });

    });

    function llenar() {
        var menu = $('#menui').val();
        var rol = $('#rol').val();

        if (rol != 0 && menu != 0) {
            cargarPrivilegiosNoAsignados(rol, menu);
            cargarPrivilegiosAsignados(rol, menu);
        }
    }

    $('#menui').change(function () {
        var rol = $('#rol').val();
        var menu = $(this).val();

        cargarPrivilegiosNoAsignados(rol, menu);
        cargarPrivilegiosAsignados(rol, menu);
    });
});