var autocompletecliente = [];

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


//llamada a metodos necesarios para el sistema
$(function () {
    var usuario = window.atob(getCookie("us"));
    //llamada a los datos para que se ejecuten al cargarse la pagina
    mostrarDatos();

    $('[data-toggle="popover"]').popover();
    $('[data-toggle="tooltip"]').tooltip();


    "use strict"
    $(document).ready(function () {
        $('#dataTables-example').DataTable({
            "order": [
                [1, "desc"]
            ]
        });
    });




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
                $("#descripcion").val(value2).trigger("change");
            }
        },
    }

    $("#nit").easyAutocomplete(options_cliente);



    $('#mdNew').click(function () {
        limpiar();
    });

    $('.MdDes').click(function () {
        $('#MdDeshabilitar').modal('toggle')
    });

    $('#bt-guardar').click(function () {
        var serie = $('#serie').val();
        var cantidad = $('#cantidad').val();
        var valor = $('#valor').val();
        var observacion = $('#observacion').val();
        var idcliente = $('#idcliente').val();


        if (serie == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: 'ADVERTENCIA',
                text: 'Debe Ingresar la serie',
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'warning',
                stack: false
            });
            $('#serie').focus();

        }
        if (observacion == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: 'ADVERTENCIA',
                text: 'Debe Ingresar la serie',
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'warning',
                stack: false
            });
            $('#observacion').focus();
        }
        else if (cantidad == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: 'ADVERTENCIA',
                text: 'Debe Ingresar la cantidad',
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'warning',
                stack: false
            });

            $('#cantidad').focus();

        }
        else if (valor == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
               heading: 'ADVERTENCIA',
               text: 'Debe Ingresar el valor',
               position: 'bottom-right',
               showHideTransition: 'plain',
               icon: 'warning',
               stack: false
            });
            $('#valor').focus();
        }
        else {
             $.ajax({
                 url: 'wsadmin_cupones.asmx/InsertarCupones',
                 data: '{serie : "' + serie + '",  observacion : "' + observacion + '",  cantidad : '+ cantidad +',  valor : '+ valor +', idcliente : '+ idcliente +',  usuario : "'+ usuario +'"}',
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
                        $('#serie').val(null);
                        $('#cantidad').val(null);
                        $('#valor').val(null);
                        $('#observacion').val(null);
                        $('#idcliente').val(null);
                        $('#nit').val(null);
                        $('#descripcion').val(null);
                        mostrarDatos();
                        $('#MdNuevo').modal('toggle')
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

    $('#bt-agregar-cupones').click(function () {

        var cantidad = $('#cantidadadd').val();
        var valor = $('#valoradd').val();

        if (cantidad == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: 'ADVERTENCIA',
                text: 'Debe Ingresar la cantidad',
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'warning',
                stack: false
            });
        } else if (valor == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: 'ADVERTENCIA',
                text: 'Debe Ingresar el valor',
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'warning',
                stack: false
            });
        } else {
            $.ajax({
               url: 'wsadmin_cupones.asmx/InsertarCuponesASerie',
               data: '{  serie:  "' + $('#serieedit').val() + '",   cantidad:  ' + cantidad + ',   valor:  '+ valor +',   id:  '+ $('#id').val() +'}',
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
                       listadetalle($('#id').val());
                       $('#cantidadadd').val(null);
                       $('#valoradd').val(null);

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


    //accion  para guardar los datos
    $('#bt-cancelar').click(function () {
        limpiar();
    })


});



//metodo para limpiar el formulario
function limpiar() {

    $('#company').val(0);
    $('#descripcion').val(null);
    $('#id').val(0);
    $('#establecimiento').val(null);
    $('#inicio').val(null);
    $('#final').val(null);
    $('#actual').val(null);
    $('#autorizacion').val(null);

    $("#fecha").val(null)

    $('#caja').val(null);
    $('#doc').val(0);
    $('#company').removeClass('is-invalid');
    $('#company').removeClass('is-valid');


    $('#establecimiento').removeClass('is-invalid is-valid');
    $('#inicio').removeClass('is-invalid is-valid');
    $('#final').removeClass('is-invalid is-valid');
    $('#actual').removeClass('is-invalid is-valid');
    $('#autorizacion').removeClass('is-invalid is-valid');
    $('#fecha').removeClass('is-invalid is-valid');
    $('#caja').removeClass('is-invalid is-valid');
    $('#doc').removeClass('is-invalid is-valid');


    $('#descripcion').removeClass('is-invalid');
    $('#descripcion').removeClass('is-valid');
    $('#bt-guardar').removeAttr('disabled', true);
    $('#bt-cancelar').removeAttr('disabled', true);
    $('#bt-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#bt-guardar').removeClass('btn-info');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-success');
}


// funcion para cargar datos en el formulario
function eliminar(id) {
    limpiar();
    $('#id').val(id);
    $('#MdDeshabilitar').modal('toggle');
    $('#bt-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#bt-guardar').removeClass('btn-success');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-info');
}



function cargarDetalle(id, nit, nombre, serie,observacion) {

    $('#nitedit').val(nit);
    $('#descripcionedit').val(nombre);
    $('#serieedit').val(serie);
    $('#observacionedit').val(observacion);

    $('#id').val(id);

    listadetalle(id);




    $('#MdDetalle').modal('toggle')
}

function listadetalle(id) {
    $.ajax({
        url: 'wsadmin_cupones.asmx/ObtenerDetalleCupones',
        data: '{id : ' + id + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            $('#tbod-datos-cupones').html(null);
            $('#tab-datos-cupones').dataTable().fnDeleteRow();
            $('#tab-datos-cupones').dataTable().fnUpdate();
            $('#tab-datos-cupones').dataTable().fnDestroy();
        },
        success: function (msg) {
            var i = 1;
            var tds = "";
            $('#tbod-datos-cupones').html(null);
            $.each(msg.d, function () {
                tds = "<tr class='odd'><td>" + i + "</td><td>" + this.serie + "</td><td>" + this.valor + "</td><td>" + this.saldo + "</td></tr>'"
                i++;

                $("#tbod-datos-cupones").append(tds);
            });

            $('#tab-datos-cupones').dataTable();
            $('[data-toggle="popover"]').popover();
        }
    });
}


//metodo utilizado para mostrar lista de datos 
function mostrarDatos() {
    limpiar();

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_cupones.asmx/ObtenerDatos',
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
                tds = "<tr class='odd'><td>" + i + "</td><td>" + this.nit + "</td><td>" + this.cliente + "</td><td>" + this.serie + "</td><td>" + this.correlativo + "</td><td> " +
                    "<span onclick='cargarDetalle(" + this.id + ",\"" + this.nit + "\",\"" + this.cliente + "\",\"" + this.serie + "\",\"" + this.observacion + "\")' class='MdDetalleDato btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' > " +
                    "<i class='material-icons'>event_note</i> " +
                    "</span> " +
                    "<span onclick='eliminar(" + this.id + ")' class='btn btn-sm btn-outline-danger' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder Inhabilitar el dato seleccionado, Esto hara que dicho dato no aparesca en ninguna acción, menu o formulario del sistema.' data-original-title='' title=''> " +
                    "<i class='material-icons'> delete_sweep </i> " +
                    "</span></td></tr>'"
                i++;

                $("#tbod-datos").append(tds);
            });

            $('#tab-datos').dataTable();
            $('[data-toggle="popover"]').popover();
        }
    });

};


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




