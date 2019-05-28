$(window).on('load', function () {

});



function UrlExists(url) {
    var http = new XMLHttpRequest();
    http.open('HEAD', url, false);
    http.send();
    return http.status != 404;
}

var usuario = window.atob(getCookie("us"));

var datos = [];
var combos = [];
var productos = [];
var accion = 0;
var id = 0;

$.ajax({
    url: 'wsadmin_articulos.asmx/ObtenerArticulosCompuestos',
    data: '',
    type: 'POST',
    contentType: 'application/json; charset=utf-8',
    success: function (msg) {
        $.each(msg.d, function () {
            var dat = { 'codigo': this.codigo, 'nombre': this.descripcion, 'id': this.id}
            combos.push(dat);
        });
    }
});


$(function () {
    cargarBodegas();

    var options = {
        data: combos,

        getValue: function (element) {
            return element.codigo
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
                var value = $("#codigo").getSelectedItemData().codigo;
                var value2 = $('#codigo').getSelectedItemData().nombre;
                var value3 = $('#codigo').getSelectedItemData().id;
            
                $("#codigo").val(value).trigger("change");
                $("#nombre").val(value2).trigger("change");
                $("#idcombo").val(value3).trigger("change");
                $('#cantidadsol').focus();
            }
        },
    }

   

    $("#codigo").easyAutocomplete(options);

    

    $('#codigo').blur(function () {
        $.ajax({
            url: 'wscombos.asmx/obtenerDetalleAnterior',
            data: '{idcombo : ' + $('#idcombo').val() + '}',
            type: 'POST',
            async: false,
            contentType: 'application/json; charset=utf-8',
            success: function (msg) {
                $.each(msg.d, function () {
                    id = this.idcombo;
                    $('#cantidad').val(this.cantidad);
                    $('#idproducto').val(this.id);
                    $('#codigoproducto').val(this.codigo);
                    $('#nomproducto').val(this.descripcion);
                    $('#precio').val(this.precio);
                    $('#bt-agregar').click();
                });

                if (msg.d.length > 0) {
                    accion = 1;
                } else {
                    accion = 0;
                }
              
            }
        });
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
                data: '{bodega: ' + $('#bodega').val() + ', codigoproducto: "' + $('#codigoproducto').val() + '" }',
                type: 'POST',
                async: false,
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
                        cargarProducto(msg.d[0].id, msg.d[0].codigo, msg.d[0].descripcion, msg.d[0].cantidad, $('#precio').val());

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

    //accion  para cancelar los datos
    $('#btn-cancelar').click(function () {
        limpiar();
    });

    $('#busqueda').keyup(function () {
        var texto = $('#busqueda').val();
        if ($('#bodega').val() > 0 && texto.length > 1) {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wstraslados.asmx/BuscarExistenciasPorBodega',
                data: '{bodega: ' + $('#bodega').val() + ', nombre: "' + texto + '"}',
                type: 'POST',
                async: false,
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $('#tbod-datos').html(null);
                    $.each(msg.d, function () {
                        var tds = "<tr class='odd'><td>" + this.codigo + "</td><td>" + this.descripcion + "</td><td>" + this.cantidad + "</td>" +
                            "<td><span data-dismiss='modal' onclick='cargarProducto(" + this.id + ",\"" + this.codigo + "\",\"" + this.descripcion + "\",\"" + this.cantidad + "\"," + this.precio + ")' class='btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='AGREGAR AL CARRITO DE COMPRAS' data-original-title='' title ='' > " +
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

    $('#bodega').change(function () {
        //consume el ws para obtener los datos
        $.ajax({
            url: 'wstraslados.asmx/ObtenerExistenciasPorBodegaSimple',
            data: '{bodega : ' + $('#bodega').val() + '}',
            type: 'POST',
            async: false,
            contentType: 'application/json; charset=utf-8',
            success: function (msg) {
                $.each(msg.d, function () {
                    var dat = { 'descripcion': this.descripcion + '[' + this.codigo + ']', 'codigo': this.codigo, 'id': this.id, 'desc': this.descripcion, 'precio': this.precio }
                    productos.push(dat);
                });
            }
        });

        var options3 = {
            data: productos,

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
                    var value4 = $('#producto').getSelectedItemData().precio;
                    $("#idproducto").val(value).trigger("change");
                    $("#codigoproducto").val(value2).trigger("change");
                    $("#nomproducto").val(value3).trigger("change");
                    $('#precio').val(value4).trigger("change");
                    $('#existencia').val();
                }
            },
        }

        $("#producto").easyAutocomplete(options3);

    });

    $('#bn-guardar').click(function () {
        if (validarForm()) {
            var url = ""
            var dat = ""
            if (accion == 0) {
                dat = '{ usuario : "' + usuario + '", listproductos : ' + JSON.stringify(datos) + ', combo: ' + $('#idcombo').val() + ', producir: ' + $('#cantidadsol').val() + '}'
                url = "Guardar" 
            } else {
                dat = '{ usuario : "' + usuario + '", listproductos : ' + JSON.stringify(datos) + ', combo: ' + $('#idcombo').val() + ', producir: ' + $('#cantidadsol').val() + ', id : '+ id +'}'
                url = "Actualizar" 
            }
            $.ajax({
                url: 'wscombos.asmx/' + url,
                data: dat ,
                type: 'POST',
                async: false,
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
});


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


                    //consume el ws para obtener los datos
                    $.ajax({
                        url: 'wstraslados.asmx/ObtenerExistenciasPorBodegaSimple',
                        data: '{bodega : ' + this.id + '}',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        success: function (msg) {
                            $.each(msg.d, function () {
                                var dat = { 'descripcion': this.descripcion + '[' + this.codigo + ']', 'codigo': this.codigo, 'id': this.id, 'desc': this.descripcion, 'precio': this.precio }
                                productos.push(dat);
                            });
                        }
                    });

                    var options3 = {
                        data: productos,

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
                                var value4 = $('#producto').getSelectedItemData().precio;
                                $("#idproducto").val(value).trigger("change");
                                $("#codigoproducto").val(value2).trigger("change");
                                $("#nomproducto").val(value3).trigger("change");
                                $('#precio').val(value4).trigger("change");
                                $('#existencia').val();
                            }
                        },
                    }

                    $("#producto").easyAutocomplete(options3);


                } else {
                    $('#bodega').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                }

            });
        }
    });
}


function validarForm() {


    var result = true

    var mensaje = 'Existen Datos que debe ingresar para poder realizar la acción solicitada';

    if ($('#codigo').val() == "" || $('#nombre').val() == "" || $('#idcombo').val() == "") {
        $('#codigo').focus();
        result = false;
    } else if ($('#cantidadsol').val() == "") {
        $('#cantidadsol').focus();
        result = false;
    }

    if (datos.length == 0) {
        result = false
        mensaje = 'Debe Ingresar al menos un producto';
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
    $('#codigo').val(null);
    $('#idcombo').val(null);
    $('#nombre').val(null);
    $('#cantidadsol').val(null);

    $('#codigoproducto').val(null);
    $('#cantidad').val(null);
    $('#producto').val(null);
    $('#idproducto').val(null);
    $('#precio').val(null);


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

function cargarProducto(id, codigo, descripcion, cantidad, precio) {
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
    else if ((parseInt($('#cantidad').val()) * parseInt($('#cantidadsol').val())) > parseInt(cantidad)) {

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
    else {


        if (!existe) {
            var linea = { 'cantidad': $('#cantidad').val(), 'codigo': codigo, 'descripcion': descripcion, 'id': id, 'precio': $('#precio').val(), 'bodega': $('#bodega').val(), 'bo': $('#bodega option:selected').text() };
            datos.push(linea);


        } else {

            if ((cantidadTotal * cantidadsol) > cantidad) {

                $('.jq-toast-wrap').remove();
                $.toast({
                    heading: '¡ERROR!',
                    text: "NO EXISTE LA SUFICIENTE CANTIDAD PARA REDUCIR LA EXISTENCIA DEL PRODUCTO TOT",
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
            var tds = '<tr><td>' + datos[i].codigo + '</td><td>' + datos[i].descripcion + '</td><td>' + datos[i].bo + '</td><td>' + datos[i].cantidad + '</td><td style="text-align: right">' + parseFloat(datos[i].precio).toFixed(2) + '</td><td style="text-align: right">' + (parseFloat(datos[i].cantidad) * parseFloat(datos[i].precio)).toFixed(2) + '</td><td><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'
            $('#tbody').append(tds);
        };


        if (total > 0) {
            td = '<tr><td> -- </td><th> <b>TOTAL</b> </th><td><center> --- </center></td><td><center> --- </center><td><center> --- </center></td><td style="text-align: right"><b>' + parseFloat(total).toFixed(2) + '</b></td><td></td></tr>'
            $('#tbody').append(td);
        }

        totalfac = total;


       

        

        $('#codigoproducto').val(null);
        $('#cantidad').val(null);
        $('#producto').val(null);
        $('#idproducto').val(null);
        $('#precio').val(null);
    }
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
