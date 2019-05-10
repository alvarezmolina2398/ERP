var user = window.atob(getCookie("us"));


$(function () {

    mostrarDatos();
    cargarBodegas();

    //accion  para guardar o actualizar los datos
    $('#bt-guardar').click(function () {
        $('#bt-guardar').attr('disabled', true);
        $('#bt-cancelar').attr('disabled', true);
        //consume el ws para obtener los datos
        $.ajax({
            url: 'wstomainventarios.asmx/Insertar',
            data: "{observacion: '" + $('#observaciones').val() + "', usuario: '" + user + "'}",
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
                $('#bt-guardar').removeClass('btn-info');
                $('#bt-guardar').removeClass('btn-success');
                $('#bt-guardar').addClass('btn-warning');
                $('#bt-guardar').html('<i class="material-icons">query_builder</i>Cargando...')
            },
            success: function (msg) {

                var arr = msg.d.split('|');


                if (arr[0] == 'SUCCESS') {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡EXITO!',
                        text: arr[1],
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'success',
                        stack: false
                    });

                    $('#MdNuevo').modal('toggle');

                } else {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡ERROR!',
                        text: arr[1],
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'error',
                        stack: false
                    });
                }

                limpiar();
                mostrarDatos();
            }
        });
    });


    $('#bodega').change(function () {
        if ($(this).val() > 0) {

            var datos = [];

            //consume el ws para obtener los datos
            $.ajax({
                url: 'wstraslados.asmx/ObtenerExistenciasPorBodega',
                data: '{bodega: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        var dat = { 'descripcion': this.descripcion + '[' + this.codigo + ']', 'codigo': this.codigo, 'id': this.id, 'desc': this.descripcion, 'cantidad': this.cantidad }
                        datos.push(dat);
                    });


                    var options = {
                        data: datos,

                        getValue: function (element) {
                            return element.descripcion
                        },

                        list: {
                            match: {
                                enabled: true
                            },
                            onSelectItemEvent: function () {
                                var value = $("#producto").getSelectedItemData().id;
                                var value2 = $('#producto').getSelectedItemData().codigo;
                                var value3 = $('#producto').getSelectedItemData().desc;
                                var value4 = $('#producto').getSelectedItemData().cantidad;
                                $("#idproducto").val(value).trigger("change");
                                $("#codigoproducto").val(value2).trigger("change");
                                $("#nomproducto").val(value3).trigger("change");
                                $('#cantidadexistente').val(value4);
                            }
                        },
                    }

                    $("#producto").easyAutocomplete(options);

                }
            });

        }
    });


    //accion  para guardar o actualizar los datos
    $('#bt-guardardetalle').click(function () {
        $('#bt-guardar').attr('disabled', true);
        $('#bt-cancelar').attr('disabled', true);

        if ($('#cantidad').val() == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: 'DEBE INGRESAR EL DATO CANTIDAD',
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        } else if ($('#bodega').val() == 0) {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: 'DEBE SELECIONAR LA BODEGA',
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        } else if ($('#idproducto').val() == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: 'EL PRODUCTO ELEGIDO NO EXISTE',
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        }
        else if ($('#producto').val() == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: 'DEBE INGRESAR UN PRODUCTO',
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        } else {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wstomainventarios.asmx/InsertarDetalle',
                data: "{observacion: '" + $('#observaciondetalle').val() + "', usuario: '" + user + "', cantidad : " + $('#cantidad').val() + ",idtoma : " + $('#toma').text() + ",bodega : " + $('#bodega').val() + ",producto :" + $('#idproducto').val() + "}",
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                beforeSend: function () {
                    $('#bt-guardar').removeClass('btn-info');
                    $('#bt-guardar').removeClass('btn-success');
                    $('#bt-guardar').addClass('btn-warning');
                    $('#bt-guardar').html('<i class="material-icons">query_builder</i>Cargando...')
                },
                success: function (msg) {

                    var arr = msg.d.split('|');


                    if (arr[0] == 'SUCCESS') {
                        $('.jq-toast-wrap').remove();
                        $.toast({
                            heading: '¡EXITO!',
                            text: arr[1],
                            position: 'bottom-right',
                            showHideTransition: 'plain',
                            icon: 'success',
                            stack: false
                        });

                        $('#observaciondetalle').val(null)
                        $('#producto').val(null);
                        $('#idproducto').val(null);
                        $('#cantidad').val(null);

                        var d = new Date();
                        var day = d.getDate();

                        var month = d.getMonth() + 1;

                        var output = (day < 10 ? '0' : '') + day + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear()

                        $('#fechadetalle').val(output);
                        $('#horadetalle').val(d.getHours() + ":" + d.getMinutes());


                    } else {
                        $('.jq-toast-wrap').remove();
                        $.toast({
                            heading: '¡ERROR!',
                            text: arr[1],
                            position: 'bottom-right',
                            showHideTransition: 'plain',
                            icon: 'error',
                            stack: false
                        });
                    }

                }
            });
        }

       
    });



    //accion  para guardar o actualizar los datos
    $('#bt-cerrar').click(function () {
        $('#bt-guardar').attr('disabled', true);
        $('#bt-cancelar').attr('disabled', true);

        //consume el ws para obtener los datos
        $.ajax({
            url: 'wstomainventarios.asmx/Cerar',
            data: "{id : " + $('#toma').text() + "}",
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
                $('#bt-guardar').removeClass('btn-info');
                $('#bt-guardar').removeClass('btn-success');
                $('#bt-guardar').addClass('btn-warning');
                $('#bt-guardar').html('<i class="material-icons">query_builder</i>Cargando...')
            },
            success: function (msg) {

                var arr = msg.d.split('|');


                if (arr[0] == 'SUCCESS') {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡EXITO!',
                        text: arr[1],
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'success',
                        stack: false
                    });

                    $('#MdDetalle').modal('toggle');
                    limpiar();
                    mostrarDatos();

                } else {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡ERROR!',
                        text: arr[1],
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'error',
                        stack: false
                    });
                }

            }
        });
    });




    $('.Mdnew').click(function () {
        $('#MdNuevo').modal('toggle');
        var d = new Date();
        var day = d.getDate();

        var month = d.getMonth() + 1;

        var output = (day < 10 ? '0' : '') + day + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear()

        $('#fecha').val(output);
        $('#hora').val(d.getHours() + ":" + d.getMinutes());

        $('#observaciones').val(null);

    });

});


function cargarenFormulario(toma) {
    $('#toma').text(toma);
    var d = new Date();
    var day = d.getDate();

    var month = d.getMonth() + 1;

    var output = (day < 10 ? '0' : '') + day + '/' + (month < 10 ? '0' : '') + month + '/' + d.getFullYear()

    $('#fechadetalle').val(output);
    $('#horadetalle').val(d.getHours() + ":" + d.getMinutes());
    $('#observaciondetalle').val(null);

    $('#MdDetalle').modal('toggle');
}

//metodo utilizado para mostrar lista de datos 
function mostrarDatos() {
   // limpiar();

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wstomainventarios.asmx/ObtenerDatos',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            $('#tbod-datos').html(null);
            $('#tab-datos').dataTable().fnDeleteRow();
            $('#tab-datos').dataTable().fnUpdate();
            $('#tab-datos').dataTable().fnDestroy();
        },
        success: function (msg) {
            var i = 1;
            var tds = "";
            $('#tbod-datos').html(null);
            $.each(msg.d, function () {
                tds = "<tr class='odd'><td>" + this.id + "</td><td>" + this.Sucursal + "</td><td>" + this.fecha + "  " + this.hora + "</td><td>" + this.usuario + "</td><td> " +
                    "<span onclick='cargarenFormulario(" + this.id + ")' class='Mdnew btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' > " +
                    "<i class='material-icons'>edit</i> " +
                    "</span> </td></tr>'"
                i++;

                $("#tbod-datos").append(tds);
            });

            $('#tab-datos').dataTable();
            $('[data-toggle="popover"]').popover();
        }
    });

};


function limpiar() {
    $('#observaciones').val(null);
    $('#bt-guardar').removeAttr('disabled', true);
    $('#bt-cancelar').removeAttr('disabled', true);
    $('#bt-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#bt-guardar').removeClass('btn-info');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-success');

}


//metodo para obtener la sesion
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


function cargarBodegas() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_bodegas.asmx/ObtenerDatosPorSucursal',
        data: '{usuario: "' + user + '"}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {

                if (this.prioridad == 1) {
                    $('#bodega').append('<option value="' + this.id + '" selected>' + this.descripcion + '</option>');
                    //consume el ws para obtener los datos
                    $.ajax({
                        url: 'wstraslados.asmx/ObtenerExistenciasPorBodega',
                        data: '{bodega: ' + this.id + '}',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        success: function (msg) {
                            var datos = [];
                            $.each(msg.d, function () {
                                var dat = { 'descripcion': this.descripcion + '[' + this.codigo + ']', 'codigo': this.codigo, 'id': this.id, 'desc': this.descripcion, 'cantidad': this.cantidad }
                                datos.push(dat);
                            });


                            var options = {
                                data: datos,

                                getValue: function (element) {
                                    return element.descripcion
                                },

                                list: {
                                    match: {
                                        enabled: true
                                    },
                                    onSelectItemEvent: function () {
                                        var value = $("#producto").getSelectedItemData().id;
                                        var value2 = $('#producto').getSelectedItemData().codigo;
                                        var value3 = $('#producto').getSelectedItemData().desc;
                                        var value4 = $('#producto').getSelectedItemData().cantidad;
                                        $("#idproducto").val(value).trigger("change");
                                        $("#codigoproducto").val(value2).trigger("change");
                                        $("#nomproducto").val(value3).trigger("change");
                                        $('#cantidadexistente').val(value4);
                                    }
                                },
                            }

                            $("#producto").easyAutocomplete(options);

                        }
                    });

                } else {
                    $('#bodega').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                }

            });
        }
    });
}