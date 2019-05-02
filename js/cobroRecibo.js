$(window).on('load', function () {
    /* footable  */
    $("#tabla-pagos").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
    /* footable  */
    $("#tabla-fac").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });

});

var pagos = [];
var facturas = [];
var autocompletecliente = [];
var existefecctivo = false;
var totalpago = 0;
var totalfactura = 0;
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


var totefectivo = 0;

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
            $("#idcliente").val(value).trigger("change");
            $("#nombre").val(value2).trigger("change");
            cargarFacturas(value);
        }
    },
}

$("#nit").easyAutocomplete(options_cliente);



$('.pn-pagos').hide();



function agregarFormulario(factura, saldo, descripcion, valor) {
    var linea = { 'id_fac': factura, 'saldo': saldo, 'valor': valor, 'descripcion': descripcion }
    facturas.push(linea)
    var total_FAC = 0;

    $('#tbody-fac').html(null);
    for (var i = 0; i < facturas.length; i++) {
        total_FAC += parseFloat(facturas[i].saldo);
        var tds = '<tr><td>' + facturas[i].descripcion + '</td><td>' + parseFloat(facturas[i].valor).toFixed(2) + '</td><td style="text-align: right">' + parseFloat(facturas[i].saldo).toFixed(2) + '</td><td onclick="eliminarFac(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

        $('#tbody-fac').append(tds);
    };

    if (total_FAC > 0) {
        var td = '<tr><td> <b>SALDO TOTAL</b> </td><th> ------ </th><td style="text-align: right"><b>' + parseFloat(total_FAC).toFixed(2) + '</b></td></tr>'
        $('#tbody-fac').append(td);

    }


    $("#tabla-fac").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });

}


function cargarFacturas(idcliente) {
    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscobrar_Recibos.asmx/ObtenerFacturas',
        data: '{idcliente: ' + idcliente +'}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $('#tbody-Fac').html(null);
            $.each(msg.d, function () {

                var dts = "<tr class='odd'><td>" + this.serie + "-" + this.firma + "</td><td>" + this.valor + "</td><td>" + this.saldo + "</td><td>" +
                    "<span data-dismiss='modal' onclick='agregarFormulario(" + this.id_fac + "," + this.saldo +",\"" + this.serie+ "-"+ this.firma + "\","+ this.valor+")' class='btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='AGREGAR AL CARRITO DE COMPRAS' data-original-title='' title ='' > " +
                    "<i class='material-icons'>shopping_cart</i> " +
                    "</span></td></tr>"

                $('#tbody-Fac').append(dts);

            });
            $('#tab-datos').dataTable();
        }
    });
}

$(function () {
    cargarMetodosdePago();
    cargarTiposTarjeta();

    //levanta modal de facturas
    $('#btn-add-fac').click(function () {
        $('#MdFac').modal('toggle');
    });

    //acciones para las tabs


    $('#tab2-btn').click(function () {
        $('#tabhome223-tab').click();
    });


    $('#tab3-btn').click(function () {
        $('#tabhome133').click();
    });

    $('#tab1-atras-btn').click(function () {
        $('#tabhome123-tab').click();
    });

    $('#tab2-atras-btn').click(function () {
        $('#tabhome223-tab').click();
    });


    //accion al cambiar el tipo de pago
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


    //accion para guardar los datos
    $('#btn-guardar').click(function () {
        var usuario = 'admin1';
        if (pagos.length == 0) {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "ES NECESARIO INGRESAR LOS METODOS DE PAGO",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        } else if (facturas.length == 0) {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "ES NECESARIO INGRESAR LAS FACTURAS",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        } else {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wscobrar_Recibos.asmx/Pagar',
                data: '{ usuario : "' + usuario + '",  total_abonar : ' + totalpago + ', idcliente : ' + $('#idcliente').val() + ', listfac : ' + JSON.stringify(facturas) + ', listpagos : ' + JSON.stringify(pagos) + '}',
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
                        $('#MdPago').modal('toggle');
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


    $('#bt-agregar-pago').click(function () {
        var tipoPago = $('#tipopago').val();


        if (tipoPago != 0) {
            if (tipoPago == 1) {
                var efectivo = $('#efectivo').val();

                if (efectivo == "") {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡ERROR!',
                        text: "ES NECESARIO INGRESAR EL VALOR EN EFECTIVO",
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'error',
                        stack: false
                    });
                } else {
                    var info = "";
                    totefectivo = totefectivo + parseFloat(efectivo);
                    info = info = "PAGO DE: " + parseFloat(totefectivo).toFixed(2);
                    if (existefecctivo) {
                        for (var i = 0; i < pagos.length; i++) {

                            if (pagos[i].tipo == 1) {
                                pagos[i].informacion = info;
                                pagos[i].valor = totefectivo;
                                existefecctivo = true;
                            }
                        }
                    } else {
                        existefecctivo = true;
                        var linea = { 'tipo': tipoPago, 'valor': totefectivo, 'informacion': info, 'tipoPagoText': $('#tipopago option:selected').text() };
                        pagos.push(linea);
                    }


                    $('#tipopago').val(0);
                }


                $('#efectivo').val(null);


            }
            else if (tipoPago == 2) {

                var nocheque = $('#nocheque').val();
                var cheque = $('#cheque').val();


                if (cheque == "" || nocheque == "") {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡ERROR!',
                        text: "ES NECESARIO INGRESAR EL VALOR Y LA DESCRIPCION",
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'error',
                        stack: false
                    });
                } else {
                    var linea = { 'tipo': tipoPago, 'valor': cheque, 'informacion': 'CHEQUE:' + nocheque, 'tipoPagoText': $('#tipopago option:selected').text(), 'cambio': 0 };
                    pagos.push(linea);
                    $('#tipopago').val(0);
                }

                $('#nocheque').val(null);
                $('#cheque').val(null);



            }
            else if (tipoPago == 3) {
                var tipo = $('#tipotarjeta').val();
                var autorizacion = $('#autorizacion').val();
                var codigo = $('#codigo').val();
                var tarjeta = $('#tarjeta').val();


                if (tipo == 0 || autorizacion == "" || tarjeta == "") {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡ERROR!',
                        text: "ES NECESARIO INGRESAR EL TIPO TARJETA, AUTORIZACION Y TARJETA",
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'error',
                        stack: false
                    });
                }
                else {
                    var informacion = "TIPO DE TARJETA: " + $('#tipotarjeta option:selected').text() + ", AUTORIZACION: " + autorizacion + ", NUMERO DE TARJETA :  " + codigo
                    var linea = { 'tipo': tipoPago, 'valor': tarjeta, 'informacion': informacion, 'tipoPagoText': $('#tipopago option:selected').text(), 'cambio': 0 };
                    pagos.push(linea);
                    $('#tipopago').val(0);
                }




                $('#tipotarjeta').val(0);
                $('#autorizacion').val(null);
                $('#codigo').val(null);
                $('#tarjeta').val(null);


            }
            else if (tipoPago == 7) {

                var formulario = $('#formulario').val();
                var valorexcersion = $('#valorexcersion').val();


                if (formulario == "" || valorexcersion == "") {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡ERROR!',
                        text: "ES NECESARIO INGRESAR EL VALOR Y LA DESCRIPCION DEL FORMULARIO",
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'error',
                        stack: false
                    });
                }
                else {
                    var linea = { 'tipo': tipoPago, 'valor': valorexcersion, 'informacion': formulario, 'tipoPagoText': $('#tipopago option:selected').text(), 'cambio': 0 };
                    pagos.push(linea);
                    $('#tipopago').val(0);
                }

                $('#formulario').val(null);
                $('#valorexcersion').val(null);

            }
            else if (tipoPago == 8) {

                var regaloinfo = $('#regaloinfo').val();
                var regalo = $('#regalo').val();


                if (regaloinfo == "" || regalo == "") {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡ERROR!',
                        text: "ES NECESARIO INGRESAR EL VALOR DEL REGALO Y LA DESCRIPCION",
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'error',
                        stack: false
                    });
                }
                else {
                    var linea = { 'tipo': tipoPago, 'valor': regalo, 'informacion': 'FORMULARIO: ' + regaloinfo, 'tipoPagoText': $('#tipopago option:selected').text(), 'cambio': 0 };
                    pagos.push(linea);
                    $('#tipopago').val(0);
                }


                $('#regaloinfo').val(null);
                $('#regalo').val(null);

            }


            var total_pago = 0;

            $('#tbody-pago').html(null);
            for (var i = 0; i < pagos.length; i++) {
                total_pago += parseFloat(pagos[i].valor);
                var tds = '<tr><td>' + pagos[i].tipoPagoText + '</td><td>' + pagos[i].informacion + '</td><td style="text-align: right">' + parseFloat(pagos[i].valor).toFixed(2) + '</td><td onclick="eliminarPago(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

                $('#tbody-pago').append(tds);
            };


            if (total_pago > 0) {
                td = '<tr><td> -- </td><th> <b>TOTAL</b> </th><td style="text-align: right"><b>' + parseFloat(total_pago).toFixed(2) + '</b></td></tr>'
                $('#tbody-pago').append(td);

            }
            totalpago = total_pago

            $("#tabla-pagos").footable({
                "paging": {
                    "enabled": true,
                    "position": "center"
                }
            });
            $('#tipopago').val(0);
            $('.pn-pagos').hide();

        } else {

            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "ES NECESARIO SELECCIONAR UN METODO DE PAGO",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        }


    });


});


function limpiar() {
    $('#nombre').val(null);
    $('#nit').val(null);
    $('#idcliente').val(null);


    $('#tbody-pago').html(null);
    $('#tbody-fac').html(null);

    $("#tabla-pagos").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
    /* footable  */
    $("#tabla-fac").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
    $('#tabhome123-tab').click();

    $('#btn-guardar').removeAttr('disabled', true);
    $('#btn-cancelar').removeAttr('disabled', true);
    $('#btn-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#btn-guardar').removeClass('btn-info');
    $('#btn-guardar').removeClass('btn-warning');
    $('#btn-guardar').addClass('btn-success');

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
                if (this.id != 6) {
                    $('#tipopago').append('<option value="' + this.id + '">' + this.descripcion + '</option>');
                }
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