$(window).on('load', function () {
    /* footable  */
    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
});



$('input[name="fecha"]').daterangepicker({
    singleDatePicker: true,
    showDropdowns: true,
    locale: {
        format: 'DD/MM/YYYY'
    },
    minYear: 1901
}, function (start, end, label) { });

var datosAutocomplete = [];
var datosAutocomplete2 = [];
//consume el ws para obtener los datos
$.ajax({
    url: 'wsadmin_articulos.asmx/ObtenerDatos',
    data: '',
    type: 'POST',
    contentType: 'application/json; charset=utf-8',
    success: function (msg) {
        $.each(msg.d, function () {
            var dat = { 'descripcion': this.descripcion + '[' + this.codigo + ']', 'codigo': this.codigo, 'id': this.id, 'desc': this.descripcion }
            datosAutocomplete.push(dat);
        });
    }
});

//consume el ws para obtener los datos
$.ajax({
    url: 'wscargar_datos.asmx/cargarProveedor',
    data: '',
    type: 'POST',
    contentType: 'application/json; charset=utf-8',
    success: function (msg) {
        $.each(msg.d, function () {
            var dat = { 'nit': this.extra, 'descripcion': this.descripcion, 'id': this.id }
            datosAutocomplete2.push(dat);
        });
    }
});

var datos = [];
var porcentageFinal;

function obtenerTotalCosto() {
    var flete = 0;
    var seguro = 0;
    var ogastos = 0;
    var credito = 0;

    var fletelocal = 0;
    var agente = 0;
    var almacenaje = 0;


    if ($('#fleteext').val() != "") {
        flete = parseFloat($('#fleteext').val());
    }

    if ($('#Seguroext').val() != "") {
        seguro = parseFloat($('#Seguroext').val());
    }

    if ($('#otrosgastosext').val() != "") {
        ogastos = parseFloat($('#otrosgastosext').val());
    }

    if ($('#Credito').val() != "") {
        credito = parseFloat($('#Credito').val());
    }

    if ($('#fletelocal').val() != "") {
        fletelocal = parseFloat($('#fletelocal').val());
    }

    if ($('#fletelocal').val() != "") {
        fletelocal = parseFloat($('#fletelocal').val());
    }
    if ($('#agenteaduana').val() != "") {
        agente = parseFloat($('#agenteaduana').val());
    }

    if ($('#Almacenaje').val() != "") {
        almacenaje = parseFloat($('#Almacenaje').val());
    }

    var total = flete + seguro + ogastos - credito + fletelocal + agente + almacenaje;
    var porcentage = (total / totalfac);
    porcentageFinal = porcentage;
    $('#Totalgasto').text(total.toFixed(2));
    $('#porcentajegasto').text((porcentage*100).toFixed(2));

    $('#totalcif').text((totalfac * (1 + porcentage)).toFixed(2));
}

function obtenerTotalImpusto() {
    var total = 0;
    var iva = 0;
    var Arancel = 0;
    if ($('#Iva').val() != "") {
        iva =  parseFloat($('#Iva').val());
    }

    if ($('#Arancel').val() != "") {
        Arancel = parseFloat($('#Arancel').val());
    }

    var total = Arancel + iva;
    $('#totalimpuesto').text(total.toFixed(2));
}


//agrega el valor en arancel
function agregarArancel(posicion, valor) {
    datos[posicion].arancel = valor;
    llenarTablas();
    obtenerTotalImpusto();
}

var totalfac = 0;

function externa() {
    $('#tab2').show();
    $('#tab1').hide();
    $('#datos2').show();
}

$('#tipocompra').val(1);

function interna() {
    $('#tab2').hide();
    $('#tab1').show();
    $('#datos2').hide();
}

//llena la tabla de datos
function llenarTablas() {
    if ($('#tipocompra').val() == 1) {

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
        //cuando es compra en extranjero
    } else {
        
        var totalArancel = 0;
        var total = 0;
        $('#tbody2').html(null);
        for (var i = 0; i < datos.length; i++) {
            total += (datos[i].cantidad * datos[i].precio);
            totalArancel += parseFloat(datos[i].arancel);
            var tds = '<tr><td>' + datos[i].codigo + '</td><td>' + datos[i].descripcion + '</td><td>' + parseFloat(datos[i].precio).toFixed(2) + '</td><td>' + datos[i].cantidad + '</td><td>' + parseFloat(datos[i].cantidad * datos[i].precio).toFixed(2) + '</td><td><input type="number" class="form-control" autocomplete="off" value="' + datos[i].arancel + '" onblur="agregarArancel(' + i + ',$(this).val())" /></td><td onclick="eliminar(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

            $('#tbody2').append(tds);
        };
        if (total > 0) {
            td = '<tr><td> -- </td><th> <b>TOTAL</b> </th><td><center> --- </center></td><td><center> --- </center></td><td><b>' + parseFloat(total).toFixed(2) + '</b></td><td><b>' + parseFloat(totalArancel).toFixed(2) + '</b></td><td></td></tr>'
            $('#tbody2').append(td);
        }
        $('#totalfob').text(parseFloat(total).toFixed(2));
        $('#Arancel').val(totalArancel);
    }

    totalfac = total;

    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });

}

$(function () {
    //oculta la tabla 2
    interna();
    var usuario = window.atob(getCookie("us"));
    cargarTipoPedido();
    CargarMonedas();
    cagarSucursal();
    cagarDepartamentoLaboral();

    var options = {
        data: datosAutocomplete,

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
                $("#idproducto").val(value).trigger("change");
                $("#codigoproducto").val(value2).trigger("change");
                $("#nomproducto").val(value3).trigger("change");
            }
        },
    }


    $('#tipocompra').change(function () {
        if ($(this).val() == 1) {
            interna();
        } else {
            externa();
        }
    });

    var options2 = {
        data: datosAutocomplete2,

        getValue: function (element) {
            return element.nit
        },


        template: {
            type: "description",
            fields: {
                description: "descripcion"
            }
        },
        list: {
            match: {
                enabled: true
            }, onSelectItemEvent: function () {
                var value = $("#nit").getSelectedItemData().descripcion;
                var value2 = $('#nit').getSelectedItemData().id;
                $("#idproveedor").val(value2).trigger("change");
                $("#nombre").val(value).trigger("change");
            }

        },
    }


    var options3 = {
        data: datosAutocomplete2,

        getValue: function (element) {
            $('#nit').val(element.nit);
            $('#idproveedor').val(element.id);
            return element.descripcion
        },


        template: {
            type: "description",
            fields: {
                description: "nit"
            }
        },
        list: {
            match: {
                enabled: true
            }, onSelectItemEvent: function () {
                var value = $("#nombre").getSelectedItemData().nit;
                var value2 = $('#nombre').getSelectedItemData().id;
                $("#idproveedor").val(value2).trigger("change");
                $("#nit").val(value).trigger("change");
            }
        },
    }

    $("#producto").easyAutocomplete(options);
    $('#nit').easyAutocomplete(options2);
    $('#nombre').easyAutocomplete(options3);




    $('#orden').blur(function () {

        if ($('#orden').val() != "") {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsorden_compra.asmx/obtenerDatosOrden',
                data: '{orden: ' + $('#orden').val() +'}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    if (msg.d.nit == null) {

                        $('.jq-toast-wrap').remove();
                        $.toast({
                            heading: 'ADVERTENCIA',
                            text: "NO EXISTE EL NUMERO DE ORDEN O ESTA YA FUE UTILIZADA",
                            position: 'bottom-right',
                            showHideTransition: 'plain',
                            icon: 'warning',
                            stack: false
                        });
                        limpiar();
                        $('#orden').focus();
                        
                    } else {
                        $('#nit').val(msg.d.nit);
                        $('#idproveedor').val(msg.d.idproveedor);
                        $('#nombre').val(msg.d.proveedor);
                        $('#sucursal').val(msg.d.sucursal);
                        $('#departamento').val(msg.d.Departamento);
                        $('#observacion').val(msg.d.observaciones);
                        $('#moneda').val(msg.d.moneda);
                        var solicitante = msg.d.empleado;
                        //consume el ws para obtener los datos
                        $.ajax({
                            url: 'wscargar_datos.asmx/CargarSolicitante',
                            data: '{cia: ' + msg.d.Departamento + '}',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            success: function (msg) {
                                $.each(msg.d, function () {
                                    $('#solicitante').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                                });

                                $('#solicitante').val(solicitante);
                            }
                        });
                        $('#tipoorden').val(msg.d.tipoorden);
                        datos = msg.d.producto;

                        llenarTablas();
                    }
                   
                            
                }
            });
        } else {

            limpiar();
        }
    });


    


    //accion para cargar la tabla
    $('#bt-agregar').click(function () {

        if ($('#cantidad').val() == "" || $('#producto').val() == "" || $('#precio').val() == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "ES  NECESARIO INGRESAR EL NOMBRE DEL PRODUCTO, CODIGO, CANTIDAD Y PRECIO",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });

        } else if ($('#idproducto').val() == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "EL PRODUCTO NO ESTA REGISTRADO EN EL SISTEMA",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });

        } else {

            var linea = { 'cantidad': $('#cantidad').val(), 'codigo': $('#codigoproducto').val(), 'descripcion': $('#nomproducto').val(), 'precio': $('#precio').val(), 'id': $('#idproducto').val(), 'arancel': 0 };
            datos.push(linea);

            llenarTablas();

            $('#codigoproducto').val(null);
            $('#cantidad').val(null);
            $('#producto').val(null);
            $('#precio').val(null);
            $('#idproducto').val(null);
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


    //accion  para guardar o actualizar los datos
    $('#btn-guardar').click(function () {
        $('#btn-guardar').attr('disabled', true);
        $('#bt-cancelar').attr('disabled', true);

        if (validarForm()) {
            var idproveedor = $('#idproveedor').val();
            var moneda = $('#moneda').val();
            var sucursal = $('#sucursal').val();
            var departamento = $('#departamento').val();
            var solicitante = $('#solicitante').val();
            var tipo = $('#tipoorden').val();
            var observacion = $('#observacion').val();
            var factura = $('#fac').val();
            var serie = $('#serie').val();
            var fehcaingreso = $('#fechaingreso').val();
            var flete = 0;
            var seguro = 0;
            var ogastos = 0;
            var credito = 0;

            var fletelocal = 0;
            var agente = 0;
            var almacenaje = 0;


            if ($('#fleteext').val() != "") {
                flete = parseFloat($('#fleteext').val());
            }

            if ($('#Seguroext').val() != "") {
                seguro = parseFloat($('#Seguroext').val());
            }

            if ($('#otrosgastosext').val() != "") {
                ogastos = parseFloat($('#otrosgastosext').val());
            }

            if ($('#Credito').val() != "") {
                credito = parseFloat($('#Credito').val());
            }

            if ($('#fletelocal').val() != "") {
                fletelocal = parseFloat($('#fletelocal').val());
            }

            if ($('#fletelocal').val() != "") {
                fletelocal = parseFloat($('#fletelocal').val());
            }
            if ($('#agenteaduana').val() != "") {
                agente = parseFloat($('#agenteaduana').val());
            }

            if ($('#Almacenaje').val() != "") {
                almacenaje = parseFloat($('#Almacenaje').val());
            }

            var iva = 0;
            var Arancel = 0;
            if ($('#Iva').val() != "") {
                iva = parseFloat($('#Iva').val());
            }

            if ($('#Arancel').val() != "") {
                Arancel = parseFloat($('#Arancel').val());
            }


            var poliza = 0;


            if ($('#Poliza').val() != "") {
                poliza = parseFloat($('#Poliza').val());
            }

            var orden = $('#orden').val();

            var tasacambio = $('#tasacambio').val();

            var dat = '';
            var url = '';


            if ($('#tipocompra').val() == 1) {
                url = 'Comprar';
                dat = '{ proveedor:' + idproveedor + ',  moneda:' + moneda + ',  sucursal:' + sucursal + ', departamento:' + departamento + ',  solicitante:' + solicitante + ',  tipoorden:' + tipo + ',  observacion:"' + observacion
                    + '",  total:' + totalfac + ',  usuario:"' + usuario + '",  listproductos: ' + JSON.stringify(datos) + ', factura: "' + factura + '", serie : "' + serie + '", orden: "' + orden +'"}';
            } else {
                var url = 'ComprarExterior'
                dat = '{ proveedor:' + idproveedor + ',  moneda:' + moneda + ',  sucursal:' + sucursal + ', departamento:' + departamento + ',  solicitante:' + solicitante + ',  tipoorden:' + tipo + ',  observacion:"' + observacion
                    + '",  total:' + totalfac + ',  usuario:"' + usuario + '",  listproductos: ' + JSON.stringify(datos) + ', factura: "' + factura + '", serie : "' + serie
                    + '",  fletee : ' + flete + ',  seguroe : ' + seguro + ',  otrosge : ' + ogastos + ',  creditoe : ' + credito + ',  iva : ' + iva + ',  fletel : ' + fletelocal + ',  agentel : ' + agente + ',  almacenajel : ' + almacenaje + ', arancelt : ' + Arancel + ',  tasac : ' + tasacambio + ', polizan : ' + poliza + ',porcentaje : ' + porcentageFinal + ',fechaingreso : "' + fehcaingreso + '",  orden : "' + orden +'"}';

                
            }
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsorden_compra.asmx/'+url,
                data: dat,
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

                        limpiar();

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


    //accion  para cancelar los datos
    $('#btn-cancelar').click(function () {
        limpiar();
    })

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
    var nit = $('#nit');
    var moneda = $('#moneda');
    var sucursal = $('#sucursal');
    var departamento = $('#departamento');
    var solicitante = $('#solicitante');
    var tipo = $('#tipoorden');


    var fleteext = $('#fleteext');
    var Seguroext = $('#Seguroext');
    var otrosgastosext = $('#otrosgastosext');
    var credito = $('#Credito');
    var iva  = $('#Iva');
    var fletelocal =$('#fletelocal');
    var agenteaduana =  $('#agenteaduana');
    var almacenaje = $('#Almacenaje');
    var tasacambio = $('#tasacambio');
    var Poliza = $('#Poliza');
    var serie =  $('#serie');
    var fac =  $('#fac');
    var arancel = $('#Arancel');


    nit.removeClass('is-invalid');
    nit.removeClass('is-valid');

    moneda.removeClass('is-invalid');
    moneda.removeClass('is-valid');

    sucursal.removeClass('is-invalid');
    sucursal.removeClass('is-valid');

    departamento.removeClass('is-invalid');
    departamento.removeClass('is-valid');

    solicitante.removeClass('is-invalid');
    solicitante.removeClass('is-valid');

    tipo.removeClass('is-invalid');
    tipo.removeClass('is-valid');


    fleteext.removeClass('is-invalid');
    fleteext.removeClass('is-valid');

    Seguroext.removeClass('is-invalid');
    Seguroext.removeClass('is-valid');

    otrosgastosext.removeClass('is-invalid');
    otrosgastosext.removeClass('is-valid');

    credito.removeClass('is-invalid');
    credito.removeClass('is-valid');

    iva.removeClass('is-invalid');
    iva.removeClass('is-valid');

    fletelocal.removeClass('is-invalid');
    fletelocal.removeClass('is-valid');

    agenteaduana.removeClass('is-invalid');
    agenteaduana.removeClass('is-valid');


    almacenaje.removeClass('is-invalid');
    almacenaje.removeClass('is-valid');

    arancel.removeClass('is-invalid');
    arancel.removeClass('is-valid');


    tasacambio.removeClass('is-invalid');
    tasacambio.removeClass('is-valid');

    Poliza.removeClass('is-invalid');
    Poliza.removeClass('is-valid');

    serie.removeClass('is-invalid');
    serie.removeClass('is-valid');

    fac.removeClass('is-invalid');
    fac.removeClass('is-valid');

    var result = true
    if (nit.val() == "") {
        nit.addClass('is-invalid');
        nit.focus();
        $('#btn-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);



        result = false;
    } else {
        nit.addClass('is-valid');
    }

    if (moneda.val() == 0) {
        moneda.addClass('is-invalid');
        moneda.focus();
        $('#btn-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);

        result = false;
    } else {
        moneda.addClass('is-valid');
    }

    if (sucursal.val() == 0) {
        sucursal.addClass('is-invalid');
        sucursal.focus();
        $('#btn-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);

        result = false;
    } else {
        sucursal.addClass('is-valid');
    }

    if (departamento.val() == 0) {
        departamento.addClass('is-invalid');
        departamento.focus();
        $('#btn-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);

        result = false;
    } else {
        departamento.addClass('is-valid');
    }

    if (solicitante.val() == 0) {
        solicitante.addClass('is-invalid');
        solicitante.focus();
        $('#btn-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);

        result = false;
    } else {
        solicitante.addClass('is-valid');
    }

    if (tipo.val() == 0) {
        tipo.addClass('is-invalid');
        tipo.focus();
        $('#btn-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);

        result = false;
    } else {
        tipo.addClass('is-valid');
    }



    if ($('#tipocompra').val() ==  2) {
        if (fleteext.val() == "") {
            fleteext.addClass('is-invalid');
            fleteext.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

        if (Seguroext.val() == "") {
            Seguroext.addClass('is-invalid');
            Seguroext.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

        if (otrosgastosext.val() == "") {
            otrosgastosext.addClass('is-invalid');
            otrosgastosext.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

        if (credito.val() == "") {
            credito.addClass('is-invalid');
            credito.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

        if (iva.val() == "") {
            iva.addClass('is-invalid');
            iva.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

        if (fletelocal.val() == "") {
            fletelocal.addClass('is-invalid');
            fletelocal.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }


        if (agenteaduana.val() == "") {
            agenteaduana.addClass('is-invalid');
            agenteaduana.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

        if (almacenaje.val() == "") {
            almacenaje.addClass('is-invalid');
            almacenaje.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }


        if (tasacambio.val() == "") {
            tasacambio.addClass('is-invalid');
            tasacambio.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

        if (Poliza.val() == "") {
            Poliza.addClass('is-invalid');
            Poliza.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

        if (serie.val() == "") {
            serie.addClass('is-invalid');
            serie.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

        if (fac.val() == "") {
            fac.addClass('is-invalid');
            fac.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

        if (arancel.val() == "") {
            arancel.addClass('is-invalid');
            arancel.focus();
            $('#btn-guardar').removeAttr('disabled', true);
            $('#bt-cancelar').removeAttr('disabled', true);

            result = false;
        }

    }


    var mensaje = 'Existen Datos que debe ingresar para poder realizar la acción solicitada';
    if (datos.length == 0) {
        result = false
        mensaje = 'Debe Ingresar al menos un producto a la orden'
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
    $('#nit').val(null);
    $('#moneda').val(0);
    $('#sucursal').val(0);
    $('#departamento').val(0);
    $('#solicitante').val(0);
    $('#tipoorden').val(0);
    $('#nombre').val(null)
    $('#observacion').val(null)
    $('#tbody').html(null);
    $('#tbody2').html(null);
    $('#fechaingreso').val(null);
    $('#orden').val(null);
    $('#fleteext').val(null);
    $('#Seguroext').val(null);
    $('#otrosgastosext').val(null)
    $('#Credito').val(null);
    $('#Iva').val(null);
    $('#fletelocal').val(null);
    $('#agenteaduana').val(null);
    $('#Almacenaje').val(null);
    $('#Arancel').val(null);
    $('#tasacambio').val(null);
    $('#Poliza').val(null);
    $('#serie').val(null);
    $('#fac').val(null);

    $('#tipocompra').val(1);

    $('#totalfob').text('0.00');
    $('#Totalgasto').text('0.00');
    $('#porcentajegasto').text('0.00');
    $('#totalcif').text('0.00');
    $('#totalimpuesto').text('0.00');

    interna();

    datos = [];

    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });


    $('input[name="fecha"]').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        locale: {
            format: 'DD/MM/YYYY'
        },
        minYear: 1901
    }, function (start, end, label) { });


    $('#nit').removeClass('is-invalid');
    $('#nit').removeClass('is-valid');


    $('#fleteext').removeClass('is-invalid');
    $('#fleteext').removeClass('is-valid');

    $('#Seguroext').removeClass('is-invalid');
    $('#Seguroext').removeClass('is-valid');

    $('#otrosgastosext').removeClass('is-invalid');
    $('#otrosgastosext').removeClass('is-valid');

    $('#Credito').removeClass('is-invalid');
    $('#Credito').removeClass('is-valid');

    $('#Iva').removeClass('is-invalid');
    $('#Iva').removeClass('is-valid');

    $('#fletelocal').removeClass('is-invalid');
    $('#fletelocal').removeClass('is-valid');

    $('#agenteaduana').removeClass('is-invalid');
    $('#agenteaduana').removeClass('is-valid');


    $('#Almacenaje').removeClass('is-invalid');
    $('#Almacenaje').removeClass('is-valid');

    $('#Arancel').removeClass('is-invalid');
    $('#Arancel').removeClass('is-valid');


    $('#tasacambio').removeClass('is-invalid');
    $('#tasacambio').removeClass('is-valid');

    $('#Poliza').removeClass('is-invalid');
    $('#Poliza').removeClass('is-valid');

    $('#solicitante').removeClass('is-invalid');
    $('#solicitante').removeClass('is-valid');


    $('#serie').removeClass('is-invalid');
    $('#serie').removeClass('is-valid');

    $('#fac').removeClass('is-invalid');
    $('#fac').removeClass('is-valid');


    $('#moneda').removeClass('is-invalid');
    $('#moneda').removeClass('is-valid');

    $('#sucursal').removeClass('is-invalid');
    $('#sucursal').removeClass('is-valid');

    $('#tipoorden').removeClass('is-invalid');
    $('#tipoorden').removeClass('is-valid');

    $('#departamento').removeClass('is-invalid');
    $('#departamento').removeClass('is-valid');

    $('#tipoorden').removeClass('is-invalid');
    $('#tipoorden').removeClass('is-valid');

    $('#btn-guardar').removeAttr('disabled', true);
    $('#bt-cancelar').removeAttr('disabled', true);
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


//funcion para cargar las companias
function CargarMonedas() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_monedas.asmx/ObtenerDatos',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#moneda').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}


function cagarSucursal() {
    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_sucursales.asmx/ObtenerDatosPorIDEmpresa',
        data: '{id: 1}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#sucursal').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}

function cagarDepartamentoLaboral() {
    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarDepartamentoLaboral',
        data: '{id: 1}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#departamento').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
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
