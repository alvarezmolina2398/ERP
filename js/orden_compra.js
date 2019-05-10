$(window).on('load', function () {
    /* footable  */
    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
});
var user = window.atob(getCookie("us"));
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
            var dat = {'descripcion' : this.descripcion + '['+this.codigo+']', 'codigo' : this.codigo, 'id' : this.id, 'desc' : this.descripcion}
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
            var dat = {'nit' : this.extra, 'descripcion' : this.descripcion, 'id': this.id}
            datosAutocomplete2.push(dat);
        });
    }
});

var datos = [];

var totalfac = 0;



function UrlExists(url) {
    var http = new XMLHttpRequest();
    http.open('HEAD', url, false);
    http.send();
    return http.status != 404;
} 

$(function () {
    $('[data-toggle="tooltip"]').tooltip();
    $('#cod-reimprimir').val(null);

    //evento enter en producto
    $('#cod-reimprimir').keypress(function (e) {
        var keycode = (e.keyCode ? e.keyCode : e.which);
        if (keycode == 13 && $(this).length > 0) {

            $.ajax({
                url: 'wsorden_compra.asmx/Reimprimir',
                data: '{orden : ' + $(this).val()  +', usuario : "'+ user +'" }',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {

                    var arr = msg.d.split('|');


                    if (arr[0] == 'SUCCESS') {

                        if (!UrlExists(arr[1])) {
                            $('.jq-toast-wrap').remove();
                            $.toast({
                                heading: '¡ERROR!',
                                text: "ESTA ORDEN DE COMPRA NO EXISTE",
                                position: 'bottom-right',
                                showHideTransition: 'plain',
                                icon: 'error',
                                stack: false
                            });
                        } else {
                            
                            window.open(arr[1], '_blank');
                        }

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

    

   

    //accion para cargar la tabla
    $('#bt-agregar').click(function () {

        if ($('#cantidad').val() == "" || $('#producto').val() == "" || $('#precio').val() == "" ) {
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
            var linea = { 'cantidad': $('#cantidad').val(), 'codigo': $('#codigoproducto').val(), 'descripcion': $('#nomproducto').val(), 'precio': $('#precio').val(), 'id': $('#idproducto').val() };
            datos.push(linea);

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
        if ($(this).val()>0) {
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
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsorden_compra.asmx/Ordenar',
                data: '{ proveedor:' + idproveedor + ',  moneda:' + moneda + ',  sucursal:' + sucursal + ', departamento:' + departamento + ',  solicitante:' + solicitante + ',  tipoorden:' + tipo + ',  observacion:"' + observacion + '",  total:' + totalfac + ',  usuario:"' + user + '",  listproductos: ' + JSON.stringify(datos) + '}',
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

                        window.open(arr[2], '_blank');

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
    $('#bt-cancelar').click(function () {
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
    $('#solicitante').html('<option value="0">Seleccione Una Opción</option>');
    $('#tipoorden').val(0);
    $('#nombre').val(null)
    $('#observacion').val(null)
    $('#tbody').html(null);

    datos = [];

    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });


    
    $('#nit').removeClass('is-invalid');
    $('#nit').removeClass('is-valid');

    $('#solicitante').removeClass('is-invalid');
    $('#solicitante').removeClass('is-valid');


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
function cargarenFormulario(id, descripcion, codigo, codigo1, codigo2, idcolor, idmarca, tipo, idclasificacion, preciogt, precioes, costo, idsubclasificacion, idsubmarca) {
    limpiar();
    $('#id').val(id);
    $('#descripcion').val(descripcion);

    $('#region').html('<option value="0">Seleccione Una Opción</option>');
    $('#sucursal').html('<option value="0">Seleccione Una Opción</option>');

    $('#codigo').val(codigo);
    $('#descripcion').val(descripcion);
    $('#tipo').val(tipo);
    $('#codigo1').val(codigo1);
    $('#codigo2').val(codigo2);
    $('#marca').val(idmarca);
    $('#submarca').val('<option value="0">Seleccione Una Opción</option>');
    $('#clasificacion').val(idclasificacion);
    $('#sub-clasificacion').val('<option value="0">Seleccione Una Opción</option>');
    $('#color').val(idcolor);
    $('#preciogt').val(preciogt);
    $('#precioes').val(precioes);
    $('#costo').val(costo);


    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarSubClasificacionAtr',
        data: '{art: ' + idclasificacion + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#sub-clasificacion').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#sub-clasificacion').val(idsubclasificacion);
        }
    });

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarSubMarcasporMarc',
        data: '{marca: ' + idmarca + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#submarca').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#submarca').val(idsubmarca);
        }
    });




    $('#MdNuevo').modal('toggle')

    $('#btn-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#btn-guardar').removeClass('btn-success');
    $('#btn-guardar').removeClass('btn-warning');
    $('#btn-guardar').addClass('btn-info');
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