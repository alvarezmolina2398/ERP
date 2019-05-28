$(window).on('load', function () {
    /* footable  */
    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });

    /* footable  */
    $("#tabla-pagos").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
});

var usuario = window.atob(getCookie("us"));

var datos = [];
var pagos = [];
var totalfac = 0;
var totaldescuento = 0;



var totalpagoextra = 0;
var autocompletecliente = [];
var existefecctivo = false;
$('.pn-pagos').hide();

$.ajax({
    url: 'wsadmin_clientes.asmx/ObtenerDatos',
    data: '',
    type: 'POST',
    contentType: 'application/json; charset=utf-8',
    success: function (msg) {
        $.each(msg.d, function () {
            var dat = { 'nit': this.nit, 'nombre': this.nombre, 'id': this.id, 'dias': this.dias, 'descuento': this.descuento }
            autocompletecliente.push(dat);
        });
    }
});

$(function () {


    $('#tab-datos').dataTable();
    var fecha = new Date();
    $('#fechaactual').text(fecha.getDate() + "/" + (fecha.getMonth() + 1) + "/" + fecha.getFullYear());
    cargarBodegas();
    cargarMetodosdePago();
    cargarTiposTarjeta();


    var options_cliente = {
        data: autocompletecliente,

        getValue: function (element) {
            return element.nit
        },
        template: {
            type: "description",
            fields: {
                description: "nombre"
            }
        },
        list: {
            match: {
                enabled: true
            },
            onSelectItemEvent: function () {
                var value = $("#nit").getSelectedItemData().id;
                var value2 = $('#nit').getSelectedItemData().nombre;
                var value3 = $('#nit').getSelectedItemData().dias;
                var value4 = $('#nit').getSelectedItemData().descuento;
                $("#idcliente").val(value).trigger("change");
                $("#nombre").val(value2).trigger("change");
                $("#descuento").val(value4).trigger("change");
                $("#diascredito").val(value3).trigger("change");
            }
        },
    }

    $("#nit").easyAutocomplete(options_cliente);

    $('#bt-CrearCliente').click(function () {
        $('#MdCrearCliente').modal('toggle')
    });

    //accion para crear los clientes
    $('#btn-Cliente').click(function () {
        var nit = $('#nitnew').val();
        var descripcion = $('#nombrenew').val();
        var direccion = $('#direccion').val();


        if (nit == "" && descripcion == "" && direccion == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "LOS CAMPOS DE NIT, NOMBRE Y DIRECCION SON NECESARIOS",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        } else {
            var telefono = ' -- ';
            var observacion = 'CLIENTE CREADO DESDE FACTURACION';
            var departamento = 1;
            var municipio = 1;
            var clasificacion = 2;
            var contacto1 = 'SIN DATO';
            var contacto2 = 'SIN DATO';
            var contacto3 = 'SIN DATO';
            var descuento = 0;
            var limite = 0
            var dia = 0;
            var correo = 'SIN DATO';

            data1 = '{ nit_clt : "' + nit + '",  Nom_clt : "' + descripcion +
                '",  Tel_Clt : "' + telefono + '",  Dire_Clt : "' + direccion + '",  id_clasif : ' + clasificacion +
                ',  id_dep : ' + departamento + ',  id_muni : ' + municipio + ',  contacto1 : "' + contacto1
                + '",  contacto2 : "' + contacto2 + '",  contacto3 : "' + contacto3 + '",  Descuento_Porc : ' + descuento +
                ',  Limite_Credito : ' + limite + ',  Dias_Credito : ' + dia + ',  Correo_Clt : "' + correo + '",  Observ_Clt : "' + observacion + '"}';


            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsadmin_clientes.asmx/InsertarRetornaID',
                data: data1,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
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
                        autocompletecliente = []
                        $.ajax({
                            url: 'wsadmin_clientes.asmx/ObtenerDatos',
                            data: '',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            success: function (msg) {
                                $.each(msg.d, function () {
                                    var dat = { 'nit': this.nit, 'nombre': this.nombre, 'id': this.id, 'dias': this.dias, 'descuento': this.descuento }
                                    autocompletecliente.push(dat);
                                });
                            }
                        });




                        options_cliente = {
                            data: autocompletecliente,

                            getValue: function (element) {
                                return element.nit
                            },
                            template: {
                                type: "description",
                                fields: {
                                    description: "nombre"
                                }
                            },
                            list: {
                                match: {
                                    enabled: true
                                },
                                onSelectItemEvent: function () {
                                    var value = $("#nit").getSelectedItemData().id;
                                    var value2 = $('#nit').getSelectedItemData().nombre;
                                    var value3 = $('#nit').getSelectedItemData().dias;
                                    var value4 = $('#nit').getSelectedItemData().descuento;
                                    $("#idcliente").val(value).trigger("change");
                                    $("#nombre").val(value2).trigger("change");
                                    $("#descuento").val(value4).trigger("change");
                                    $("#diascredito").val(value3).trigger("change");
                                }
                            },
                        }

                        $("#nit").easyAutocomplete(options_cliente);


                        $('#nit').val(nit);
                        $('#nombre').val(descripcion);
                        $('#idcliente').val(arr[2]);

                        $('#nitnew').val(null);
                        $('#nombrenew').val(null);
                        $('#direccion').val(null);
                        $('#descuento').val(0);
                        $('#diascredito').val(0);
                        $('#MdCrearCliente').modal('toggle');

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

    $('#tipopago').change(function () {
        $('.pn-pagos').hide();


        if ($(this).val() != 0) {
            if ($(this).val() == 1) {
                $('#pn-efectivo').show();
                $('#efectivo').focus();
            } else if ($(this).val() == 2) {
                $('#pn-cheque').show();
                $('#nocheque').focus();
            } else if ($(this).val() == 3) {
                $('#pn-tarjeta').show();
                $('#tipotarjeta').focus();
            } else if ($(this).val() == 6) {
                $('#pn-credito').show();
                $('#credito').focus();
            } else if ($(this).val() == 7) {
                $('#pn-excension').show();
                $('#formulario').focus();
            } else if ($(this).val() == 8) {
                $('#pn-regalo').show();
                $('#regaloinfo').focus();
            }
        }

        $('#efectivo').val(null);
        $('#nocheque').val(null);
        $('#cheque').val(null);
        $('#tipotarjeta').val(0);
        $('#autorizacion').val(null);
        $('#codigo').val(null);
        $('#tarjeta').val(null);
        $('#credito').val(null);
        $('#formulario').val(null);
        $('#valorexcersion').val(null);
        $('#regaloinfo').val(null);
        $('#regalo').val(null);
    });


    $('#btn-Pagar').click(function () {
        $('#btn-guardar').attr('disabled', true);
        $('#btn-cancelar').attr('disabled', true);

        if (validarForm()) {

            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsventasinrecibo.asmx/Vender',
                data: '{ usuario : "' + usuario + '",  total : ' + totalfac + ',  descuento : ' + totaldescuento + ',  idcliente : ' + $('#idcliente').val() + ',  diascredito : ' + $('#diascredito').val() + ',  listproductos : ' + JSON.stringify(datos) + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                beforeSend: function () {
                    $('#btn-guardar').removeClass('btn-info');
                    $('#btn-guardar').removeClass('btn-success');
                    $('#btn-guardar').addClass('btn-warning');
                    $('#btn-guardar').html('<i class="material-icons">query_builder</i>Cargando...')
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

                       // window.open(arr[2], '_blank');
                        limpiar();

                    } else {
                        $('#btn-guardar').removeAttr('disabled', true);
                        $('#btn-cancelar').removeAttr('disabled', true);
                        $('#btn-guardar').html('<i class="material-icons">add</i>Guardar');
                        $('#btn-guardar').removeClass('btn-info');
                        $('#btn-guardar').removeClass('btn-warning');
                        $('#btn-guardar').addClass('btn-success');


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

    $('#bt-buscar').click(function () {
        
        $('#busqueda').val(null);
        $('#busqueda').focus();
        $('#tbod-datos').html(null);
        $('#tab-datos').dataTable();
        $('#Mdbuscar').modal('toggle')
    });

    $('#busqueda').keyup(function () {
        var texto = $('#busqueda').val();
        if ($('#bodega').val() > 0 && texto.length > 1) {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wstraslados.asmx/BuscarExistenciasPorBodega',
                data: '{bodega: ' + $('#bodega').val() + ', nombre: "' + texto + '"}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $('#tbod-datos').html(null);
                    $.each(msg.d, function () {
                        var tds = "<tr class='odd'><td>" + this.codigo + "</td><td>" + this.descripcion + "</td><td>" + this.cantidad + "</td>" +
                            "<td><span data-dismiss='modal' onclick='cargarProducto(" + this.id + ",\"" + this.codigo + "\",\"" + this.descripcion + "\",\"" + this.cantidad + "\"," + this.precio + "," + this.tipo + ")' class='btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='AGREGAR AL CARRITO DE COMPRAS' data-original-title='' title ='' > " +
                            "<i class='material-icons'>shopping_cart</i> " +
                            "</span></td></tr>"

                        $('#tbod-datos').append(tds);

                    });


                    $('#tab-datos').dataTable();
                    $('[data-toggle="popover"]').popover();

                }
            });

        } else {
            $('#tbod-datos').html(null);
            $('#tab-datos').dataTable();
        }
    });

    //accion para cargar la tabla
    $('#bt-agregar').click(function () {

        if ($('#bodega').val() == 0) {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "DEBE SELECCIONAR LA BODEGA",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        } else {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wstraslados.asmx/ObtenerExistenciasPorCodigo',
                data: '{bodega: ' + $('#bodega').val() + ', codigoproducto: "' + $('#producto').val() + '" }',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    if (msg.d.length == 0) {
                        $('.jq-toast-wrap').remove();
                        $.toast({
                            heading: '¡ERROR!',
                            text: "EL PRODUCTO NO ESTA REGISTRADO EN LA BASE DE DATOS",
                            position: 'bottom-right',
                            showHideTransition: 'plain',
                            icon: 'error',
                            stack: false
                        });
                    } else {
                        cargarProducto(msg.d[0].id, msg.d[0].codigo, msg.d[0].descripcion, msg.d[0].cantidad, msg.d[0].precio);
                    }
                }
            });

        }

    });

    //accion  al momento de acepatar elminar
    $('#bt-eliminar').click(function () {
        $('#bt-eliminar').attr('disabled', true);
        $('#bt-no').attr('disabled', true);

    });

    $('#departamento').change(function () {
        if ($(this).val() > 0) {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wscargar_datos.asmx/CargarSolicitante',
                data: '{cia: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        $('#solicitante').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                    });
                }
            });
        }
    });

    //accion  para cancelar los datos
    $('#btn-cancelar').click(function () {
        limpiar();
    });


    //evento enter en producto
    $('#producto').keypress(function (e) {
        var keycode = (e.keyCode ? e.keyCode : e.which);

        if ($('#bodega').val() == 0) {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "DEBE SELECCIONAR LA BODEGA",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        } else {
            if (keycode == 13) {
                //consume el ws para obtener los datos
                $.ajax({
                    url: 'wstraslados.asmx/ObtenerExistenciasPorCodigo',
                    data: '{bodega: ' + $('#bodega').val() + ', codigoproducto: "' + $('#producto').val() + '" }',
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    success: function (msg) {
                        if (msg.d.length == 0) {
                            $('.jq-toast-wrap').remove();
                            $.toast({
                                heading: '¡ERROR!',
                                text: "EL PRODUCTO NO ESTA REGISTRADO EN LA BASE DE DATOS",
                                position: 'bottom-right',
                                showHideTransition: 'plain',
                                icon: 'error',
                                stack: false
                            });
                        } else {
                            cargarProducto(msg.d[0].id, msg.d[0].codigo, msg.d[0].descripcion, msg.d[0].cantidad, msg.d[0].precio, msg.d[0].tipo);
                        }
                    }
                });
            }
        }

    });


    //acciion para producir
    $('#btn-producir').click(function () {
        var codigo = $('#tx-codproducir').text();
        var descripcion = $('#tx-descproducir').text();
        var precio = $('#tx-precioproducir').text();
        var cantidad = $('#cantidad').val();
        var id = $('#tx-idproducir').text()

        var producir = $('#tx-canproducir').text();
        $.ajax({
            url: 'wscombos.asmx/ProducirCombo',
            data: '{combo : ' + id + ', usuario : "' + usuario + '",  producir : ' + producir + ',  tipo : 2}',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function (msg) {
                var arr = msg.d.split('|');

                if (arr[0] == 'SUCCESS') {

                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡EXITO!',
                        text: "PRODUCCION GENERADA EXITOSAMENTE",
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'info',
                        stack: false
                    });

                    $('#MdProducir').modal('toggle');

                    cargarProducto(id, codigo, descripcion, cantidad, precio);
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

});


//funcion para cargar las de
function cargarClasificacion() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarClasificacion',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#clasificacion').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}

//funcion para cargar las de
function cargarTipoPedido() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarTipoPedido',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#tipoorden').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}


function validarForm() {


    var result = true

    var mensaje = 'Existen Datos que debe ingresar para poder realizar la acción solicitada';

    if ($('#idcliente').val() == "" || $('#nit').val() == "") {
        $('#nit').focus();
    }

    if (datos.length == 0) {
        result = false
        mensaje = 'Debe Ingresar al menos un producto a la orden';
        $('#btn-guardar').removeAttr('disabled', true);
        $('#btn-cancelar').removeAttr('disabled', true);

    }


    if ((totalfac - totaldescuento) > parseFloat($('#pago').text())) {
        result = false
        mensaje = 'EL PAGO NO ES SUFICIENTE PARA REALIZAR LA VENTA ';
        $('#btn-guardar').removeAttr('disabled', true);
        $('#btn-cancelar').removeAttr('disabled', true);
    }

    if (!result) {
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: mensaje,
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });
    }
    return result;
}

//metodo para limpiar el formulario
function limpiar() {
    $('#tbody').html(null);
    $('#descuento').val(null);
    $('#diascredito').val(null);
    $('#nombre').val(null);
    $('#nit').val(null);
    $('#idcliente').val(null);


    $('#tbod-datos').html(null);
    $('#tab-datos').dataTable();

    datos = [];
    pagos = [];

    totalfac = 0;
    totaldescuento = 0;
    totefectivo = 0;

    totefectivo = 0;
    totalpagoextra = 0;

    totalcheque = 0;
    totaltarjeta = 0;
    totalregalo = 0;
    totalcredito = 0;
    totalexcersion = 0;

    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });


    $('#btn-guardar').removeAttr('disabled', true);
    $('#btn-cancelar').removeAttr('disabled', true);
    $('#btn-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#btn-guardar').removeClass('btn-info');
    $('#btn-guardar').removeClass('btn-warning');
    $('#btn-guardar').addClass('btn-success');
}

// funcion para cargar datos en el formulario
function eliminar(id) {
    var data = datos.splice(id, 1)
    $('.jq-toast-wrap').remove();
    $.toast({
        heading: '¡Informacion!',
        text: "SE HA REMOVIDO UN PRODUCTO DE LA TABLA",
        position: 'bottom-right',
        showHideTransition: 'plain',
        icon: 'info',
        stack: false
    });



    var total = 0;
    $('#tbody').html(null);
    for (var i = 0; i < datos.length; i++) {
        total += (datos[i].cantidad * datos[i].precio);
        var tds = '<tr><td>' + datos[i].codigo + '</td><td>' + datos[i].descripcion + '</td><td>' + parseFloat(datos[i].precio).toFixed(2) + '</td><td>' + datos[i].cantidad + '</td><td>' + parseFloat(datos[i].cantidad * datos[i].precio).toFixed(2) + '</td><td onclick="eliminar(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

        $('#tbody').append(tds);
    };
    if (total > 0) {
        td = '<tr><td> -- </td><th> <b>TOTAL</b> </th><td><center> --- </center></td><td><center> --- </center></td><td><b>' + parseFloat(total).toFixed(2) + '</b></td><td></td></tr>'
        $('#tbody').append(td);
    }
    totalfac = total;


    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
}

function cargarBodegas() {
    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_bodegas.asmx/ObtenerDatosPorSucursal',
        data: '{usuario: "' + usuario + '"}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {

                if (this.prioridad == 1) {
                    $('#bodega').append('<option value="' + this.id + '" selected>' + this.descripcion + '</option>');
                    $('#bodega').attr('disabled', true);
                } else {
                    $('#bodega').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                }

            });
        }
    });
}

function cargarMetodosdePago() {
    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarTiposPago',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#tipopago').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}

function cargarTiposTarjeta() {
    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarTiposTarjetas',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#tipotarjeta').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}


function cargarProducto(id, codigo, descripcion, cantidad, precio, tipo) {
    var cantidadTotal = 0;
    var existe = false;
    var posicion = 0;


    $('#precio').val(precio);


    for (var i = 0; i < datos.length; i++) {


        if (datos[i].codigo == codigo && datos[i].bodega == $('#bodega').val()) {

            cantidadTotal = parseInt(datos[i].cantidad) + parseInt($('#cantidad').val());
            posicion = i;
            existe = true;
        }

    };

    if ($('#cantidad').val() == "") {
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: '¡ERROR!',
            text: "INGRESE LA CANTIDAD DE PRODUCTOS A VENDER",
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'error',
            stack: false
        });

    }
    else if (parseInt($('#cantidad').val()) > parseInt(cantidad)) {


        if (tipo == 1) {

            $('#tx-idproducir').text(id);
            $('#tx-canproducir').text(parseInt($('#cantidad').val()) - parseInt(cantidad));
            $('#tx-codproducir').text(codigo);
            $('#tx-descproducir').text(descripcion);
            $('#tx-precioproducir').text(precio);
            $('#MdProducir').modal('toggle');

        }
        else {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "NO EXISTE LA SUFICIENTE CANTIDAD PARA REDUCIR LA EXISTENCIA DEL PRODUCTO",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });

        }
    }
    else {


        if (!existe) {
            var linea = { 'cantidad': $('#cantidad').val(), 'codigo': codigo, 'descripcion': descripcion, 'id': id, 'precio': $('#precio').val(), 'bodega': $('#bodega').val(), 'bo': $('#bodega option:selected').text() };
            datos.push(linea);


        } else {

            if (cantidadTotal > cantidad) {

                $('.jq-toast-wrap').remove();
                $.toast({
                    heading: '¡ERROR!',
                    text: "NO EXISTE LA SUFICIENTE CANTIDAD PARA REDUCIR LA EXISTENCIA DEL PRODUCTO",
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'error',
                    stack: false
                });

            } else {
                datos[posicion].cantidad = cantidadTotal;
            }

        }



        var total = 0;
        $('#tbody').html(null);
        for (var i = 0; i < datos.length; i++) {
            total += parseFloat(datos[i].cantidad) * parseFloat(datos[i].precio);
            var tds = '<tr><td>' + datos[i].codigo + '</td><td>' + datos[i].descripcion + '</td><td>' + datos[i].bo + '</td><td>' + datos[i].cantidad + '</td><td style="text-align: right">' + parseFloat(datos[i].precio).toFixed(2) + '</td><td style="text-align: right">' + (parseFloat(datos[i].cantidad) * parseFloat(datos[i].precio)).toFixed(2) + '</td><td onclick="eliminar(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

            $('#tbody').append(tds);
        };



        var descuento = 0;



        if ($('#descuento').val() != "") {
            descuento = parseFloat($('#descuento').val()) / 100;
        }

        totaldescuento = parseFloat(total * descuento).toFixed(2);


        if (total > 0) {
            td = '<tr><td> -- </td><th> <b>DESCUENTO</b> </th><td><center> --- </center></td><td><center> --- </center><td><center> --- </center></td><td style="text-align: right"><b> - ' + totaldescuento + '</b></td><td></td></tr>'
            $('#tbody').append(td);
        }


        if (total > 0) {
            td = '<tr><td> -- </td><th> <b>TOTAL</b> </th><td><center> --- </center></td><td><center> --- </center><td><center> --- </center></td><td style="text-align: right"><b>' + parseFloat(total - totaldescuento).toFixed(2) + '</b></td><td></td></tr>'
            $('#tbody').append(td);
        }

        totalfac = total;

        $(".footable").footable({
            "paging": {
                "enabled": true,
                "position": "center"
            }
        });

        $('#codigoproducto').val(null);
        $('#cantidad').val(null);
        $('#producto').val(null);
        $('#idproducto').val(null);
        $('#precio').val(null);
    }
}

function eliminarPago(id, tipo, valor) {
    pagos.splice(id, 1)

    if (tipo == 1) {
        totefectivo = 0;

    } else {
        var info = "";
        for (var i = 0; i < pagos.length; i++) {
            if (pagos[i].tipo = 1) {

                var camb = 0;
                if (parseFloat($('#cambio').text()) >= parseFloat(valor)) {
                    var resta = parseFloat($('#cambio').text()) - parseFloat(valor);
                    camb = parseFloat(pagos[i].valor) - resta;
                } else {
                    camb = 0;
                }

                if (camb < 0) {
                    info = "DINERO EN EFECTIVO: " + pagos[i].valor
                    pagos[i].cambio = 0;
                } else {
                    info = "DINERO EN EFECTIVO: " + pagos[i].valor + " CAMBIO " + camb;
                    pagos[i].cambio = camb;
                }
                pagos[i].informacion = info;
            }
        }


        totalpagoextra = totalpagoextra - valor
    }


    $('.jq-toast-wrap').remove();
    $.toast({
        heading: '¡Informacion!',
        text: "SE HA REMOVIDO UN TIPO DE PAGO DE LA TABLA",
        position: 'bottom-right',
        showHideTransition: 'plain',
        icon: 'info',
        stack: false
    });



    var total_pago = 0;

    $('#tbody-pago').html(null);
    for (var i = 0; i < pagos.length; i++) {
        total_pago += parseFloat(pagos[i].valor);
        var tds = '<tr><td>' + pagos[i].tipoPagoText + '</td><td>' + pagos[i].informacion + '</td><td style="text-align: right">' + parseFloat(pagos[i].valor).toFixed(2) + '</td><td onclick="eliminarPago(' + i + ',' + pagos[i].tipo + ',' + pagos[i].valor + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

        $('#tbody-pago').append(tds);
    };


    if (total_pago > 0) {
        td = '<tr><td> -- </td><th> <b>TOTAL</b> </th><td style="text-align: right"><b>' + parseFloat(total_pago).toFixed(2) + '</b></td></tr>'
        $('#tbody-pago').append(td);


        $('#pago').text(parseFloat(total_pago).toFixed(2));

        if (parseFloat(total_pago) >= parseFloat(totalfac - totaldescuento)) {
            $('#lb-pago').addClass('text-success');
            $('#lb-cambio').addClass('text-success');
            $('#lb-pago').removeClass('text-danger');
            $('#lb-cambio').removeClass('text-danger');
            $('#cambio').text(parseFloat(total_pago - (totalfac - totaldescuento)).toFixed(2));
        } else {
            $('#lb-pago').removeClass('text-success');
            $('#lb-cambio').removeClass('text-success');
            $('#lb-pago').addClass('text-danger');
            $('#lb-cambio').addClass('text-danger');
            $('#cambio').text('INSUFICIENTE');
        }


    } else {
        $('#cambio').text('0.00');
        $('#pago').text('0.00');
    }


    $("#tabla-pagos").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
    $('#tipopago').val(0);
    $('.pn-pagos').hide();
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
