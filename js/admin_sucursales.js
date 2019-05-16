//llamada a metodos necesarios para el sistema
$(function () {
    //llamada a los datos para que se ejecuten al cargarse la pagina
    mostrarDatos();
    cargarCompanias();
    CargarMonedas();
    
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

    $('.Mdnew').click(function () {
        $('#MdNuevo').modal('toggle')
    });


    $('#mdNew').click(function () {
        limpiar();
    });

    $('.MdDes').click(function () {
        $('#MdDeshabilitar').modal('toggle')
    });


    //accion para cargar los tipos de facturacion
    $('#company').change(function () {
        $('#region').html('<option value="0">Seleccione Una Opción</option>')
        $('#correlativo').html('<option value="0">Seleccione Una Opción</option>');
        $('#correlativoNota').html('<option value="0">Seleccione Una Opción</option>');
        $('#correlativoFacCopia').html('<option value="0">Seleccione Una Opción</option>');
        $('#correlativoNotaCopia').html('<option value="0">Seleccione Una Opción</option>');
        $('#correlativoDebito').html('<option value="0">Seleccione Una Opción</option>');
        $('#correlativoAbono').html('<option value="0">Seleccione Una Opción</option>');
        $('#correlativosCajaElectronicosCopia').html('<option value="0">Seleccione Una Opción</option>');
        $('#correlativoAbonoCopia').html('<option value="0">Seleccione Una Opción</option>');
        $('#correlativosCajaElectronicos').html('<option value="0">Seleccione Una Opción</option>');
        $('#correlativoDebitoCopia').html('<option value="0">Seleccione Una Opción</option>');
        if ($(this).val() != 0) {
            CargarFacturacionElectronica($(this).val());
            CargarFacturacionNotaCredito($(this).val());
            CargarFacturacionDebito($(this).val());
            CargarFacturacionCopia($(this).val());
            CargarFacturacionAbono($(this).val());
            CargarFacturacionNotaCreditoCopia($(this).val());
            CargarFacturacionCaja($(this).val());
            CargarFacturacionCajaCopia($(this).val());
            CargarFacturacionDebitoCopia($(this).val());
            CargarFacturacionAbonoCopio($(this).val());
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsadmin_regiones.asmx/ObtenerDatosPorID',
                data: '{id: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        $('#region').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                    });
                }
            });

        }


    });


    //accion  para guardar o actualizar los datos
    $('#bt-guardar').click(function () {
        $('#bt-guardar').attr('disabled', true);
        $('#bt-cancelar').attr('disabled', true);
        var company = $('#company');
        var descripcion = $('#descripcion');
        var region = $('#region');
        var direccion = $('#direccion');
        var correlativo = $('#correlativo');
        var telefono = $('#telefono');
        var moneda = $('#monedas');
        var correlativoNota = $('#correlativoNota');
        var correlativoCopia = $('#correlativoFacCopia');
        var correlativoDebito = $('#correlativoDebito');
        var correlativoAbono = $('#correlativoAbono');
        var correlativoAbonoCopia = $('#correlativoAbonoCopia');
        var correlativoDebitoCopia = $('#correlativoDebitoCopia');
        var correlativoRecibo = $('#correlativosCajaElectronicos');
        var correlativoReciboCopia = $('#correlativosCajaElectronicosCopia');
        var correlativoNotaCopia = $('#correlativoNotaCopia');
        var id = $('#id').val();
        var codigo = $('#codigo');

        if (validarForm()) {
            var data1 = '';
            var url1 = ''
            if (id != 0) {
                url1 = 'Actualizar'
                data1 = '{id_empresa : ' + company.val() + ',  id_region : ' + region.val() + ',  codigo : "' + codigo.val() + '",  descripcion : "' + descripcion.val() + '",  id_correlativo : ' + correlativo.val() + ', id_correlativoNota : ' + correlativoNota.val() + ',  id_correlativoDebito : ' + correlativoDebito.val() + ',  moneda : ' + moneda.val() + ',  direccion : "' + direccion.val() + '", id_correlativoCopia : ' + correlativoCopia.val() + ', id_correlativoAbonoCopia : ' + correlativoAbonoCopia.val() + ',id_correlativoNotaCopia : ' + correlativoNotaCopia.val() + ', id_correlativoDebitoCopia : ' + correlativoDebitoCopia.val() + ', id_correlativoRecibo : ' + correlativoRecibo.val() + ', id_correlativoReciboCopia : ' + correlativoReciboCopia.val() + ',  id_correlativoAbono : ' + correlativoAbono.val() + ',   telefono : "'+ telefono.val()+'", id : '+ id +'}';
            } else {
                url1 = 'Insertar'
                data1 = '{id_empresa : ' + company.val() + ',  id_region : ' + region.val() + ',  codigo : "' + codigo.val() + '",  descripcion : "' + descripcion.val() + '",  id_correlativo : ' + correlativo.val() + ', id_correlativoNota : ' + correlativoNota.val() + ',  id_correlativoDebito : ' + correlativoDebito.val() + ',  moneda : ' + moneda.val() + ',  direccion : "' + direccion.val() + '",  id_correlativoCopia : ' + correlativoCopia.val() + ', id_correlativoAbonoCopia : ' + correlativoAbonoCopia.val() + ',id_correlativoNotaCopia : ' + correlativoNotaCopia.val() +', id_correlativoDebitoCopia : ' + correlativoDebitoCopia.val() + ', id_correlativoRecibo : ' + correlativoRecibo.val() + ', id_correlativoReciboCopia : ' + correlativoReciboCopia.val() + ',  id_correlativoAbono : ' + correlativoAbono.val() + ',  telefono : "' + telefono.val() + '"}';
            }

            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsadmin_sucursales.asmx/' + url1,
                data: data1,
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
        }
    })



    //accion  al momento de acepatar elminar
    $('#bt-eliminar').click(function () {
        $('#bt-eliminar').attr('disabled', true);
        $('#bt-no').attr('disabled', true);


        var id = $('#id').val()
        //consume el ws para obtener los datos
        $.ajax({
            url: 'wsadmin_sucursales.asmx/Inhabilitar',
            data: '{id: ' + id + '}',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
                $('#bt-eliminar').removeClass('btn-success');
                $('#bt-eliminar').addClass('btn-warning');
                $('#bt-eliminar').html('<i class="material-icons">query_builder</i>Cargando...')
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

                    $('#MdDeshabilitar').modal('toggle');

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

                $('#bt-eliminar').removeAttr('disabled', true);
                $('#bt-no').removeAttr('disabled', true);

                $('#bt-eliminar').removeClass('btn-warning');
                $('#bt-eliminar').addClass('btn-success');
                $('#bt-eliminar').html('<i class="material-icons">done</i>Si')

                limpiar();
                mostrarDatos();

            }
        });

    });



    //accion  para guardar los datos
    $('#bt-cancelar').click(function () {
        limpiar();
    })



});


function validarForm() {
    
    var company = $('#company');
    var descripcion = $('#descripcion');
    var region = $('#region');
    var direccion = $('#direccion');
    var correlativo = $('#correlativo');
    var telefono = $('#telefono');
    var monedas = $('#monedas');
    var correlativoNota = $('#correlativoNota');
    var correlativoCopia = $('#correlativoFacCopia');
    var correlativoDebito = $('#correlativoDebito');
    var correlativoAbono = $('#correlativoAbono');


    company.removeClass('is-invalid');
    company.removeClass('is-valid');

    descripcion.removeClass('is-invalid');
    descripcion.removeClass('is-valid');


    region.removeClass('is-invalid');
    region.removeClass('is-valid');

    direccion.removeClass('is-invalid');
    direccion.removeClass('is-valid');

    monedas.removeClass('is-invalid');
    monedas.removeClass('is-valid');

    correlativo.removeClass('is-invalid');
    correlativo.removeClass('is-valid');


    correlativoNota.removeClass('is-invalid');
    correlativoNota.removeClass('is-valid');

    correlativoCopia.removeClass('is-invalid');
    correlativoCopia.removeClass('is-valid');

    correlativoDebito.removeClass('is-invalid');
    correlativoDebito.removeClass('is-valid');

    correlativoAbono.removeClass('is-invalid');
    correlativoAbono.removeClass('is-valid');

    telefono.removeClass('is-invalid');
    telefono.removeClass('is-valid');


    //valida las monedas
    var result = true

    if (monedas.val() == 0) {
        monedas.addClass('is-invalid');
        monedas.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });
        result = false;
    } else {
        monedas.addClass('is-valid');
    }


    //valida la empresa
    var result = true
    if (company.val() == 0) {
        company.addClass('is-invalid');
        company.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        company.addClass('is-valid');
    }

    //valida la region
    if (region.val() == 0) {
        region.addClass('is-invalid');
        region.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        region.addClass('is-valid');
    }



    //valida el codigo
    if (direccion.val() == "") {
        direccion.addClass('is-invalid');
        direccion.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });

        result = false;
    } else {
        direccion.addClass('is-valid');
    }

    //valida el nombre
    if (descripcion.val() == "") {
        descripcion.addClass('is-invalid');
        descripcion.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });

        result = false;
    } else {
        descripcion.addClass('is-valid');
    }


    //valida el correlativo
    if (correlativo.val() == 0 && correlativoCopia.val() == 0) {
        correlativo.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Debe seleccionar al menos un tipo de facturación (FACTURACIÓN ELECTRÓNICA o FACTURACIÓN COPIA) ',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    }


    //valida el numero de telofono
    if (telefono.val() == "") {
        telefono.addClass('is-invalid');
        telefono.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });

        result = false;
    } else {
        telefono.addClass('is-valid');
    }



    return result;

}





//metodo para limpiar el formulario
function limpiar() {
    //quita valores de los cambios
    $('#company').val(0);
    $('#descripcion').val(null);
    $('#region').html('<option value="0">Seleccione Una Opción</option>');
    $('#id').val(0);
    $('#direccion').val(null);
    $('#correlativo').html('<option value="0">Seleccione Una Opción</option>');
    $('#telefono').val(null);
    $('#monedas').val(0);
    $('#codigo').val(null);
    $('#correlativo').html('<option value="0">Seleccione Una Opción</option>');
    $('#correlativoNota').html('<option value="0">Seleccione Una Opción</option>');
    $('#correlativoFacCopia').html('<option value="0">Seleccione Una Opción</option>');
    $('#correlativoNotaCopia').html('<option value="0">Seleccione Una Opción</option>');
    $('#correlativoDebito').html('<option value="0">Seleccione Una Opción</option>');
    $('#correlativoAbono').html('<option value="0">Seleccione Una Opción</option>');
    $('#correlativosCajaElectronicosCopia').html('<option value="0">Seleccione Una Opción</option>');
    $('#correlativoAbonoCopia').html('<option value="0">Seleccione Una Opción</option>');
    $('#correlativosCajaElectronicos').html('<option value="0">Seleccione Una Opción</option>');
    $('#correlativoDebitoCopia').html('<option value="0">Seleccione Una Opción</option>');
    $('#multiCollapseExample1').removeClass('show');
    //quita mensajes de las validaciones en el formulario
    $('#company').removeClass('is-invalid');
    $('#company').removeClass('is-valid');

    $('#descripcion').removeClass('is-invalid');
    $('#descripcion').removeClass('is-valid');

    $('#region').removeClass('is-invalid');
    $('#region').removeClass('is-valid');

    $('#direccion').removeClass('is-invalid');
    $('#direccion').removeClass('is-valid');

    $('#correlativo').removeClass('is-invalid');
    $('#correlativo').removeClass('is-valid');


    $('#correlativoNota').removeClass('is-invalid');
    $('#correlativoNota').removeClass('is-valid');


    $('#correlativoFacCopia').removeClass('is-invalid');
    $('#correlativoFacCopia').removeClass('is-valid');

    $('#correlativoDebito').removeClass('is-invalid');
    $('#correlativoDebito').removeClass('is-valid');


    $('#correlativoAbono').removeClass('is-invalid');
    $('#correlativoAbono').removeClass('is-valid');


    $('#telefono').removeClass('is-invalid');
    $('#telefono').removeClass('is-valid');

    $('#monedas').removeClass('is-invalid');
    $('#monedas').removeClass('is-valid');

    //resetea ellos botones a estado orgiginal
    $('#bt-guardar').removeAttr('disabled', true);
    $('#bt-cancelar').removeAttr('disabled', true);
    $('#bt-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#bt-guardar').removeClass('btn-info');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-success');
}


// funcion para cargar datos en el formulario
function cargarenFormulario(id, codigo, descripcion, idcompania, moneda, direccion, region, serieelec, serieCopia, serieNota, serieDebito, serieAbono, telefono, serieNotaCopia, serieDebitoCopia, serieAbonoCopia, serieCaja, serieCajaCopia) {
    limpiar();

    //agrega los valores al formulario
    $('#id').val(id);
    $('#descripcion').val(descripcion);
    $('#company').val(idcompania);
    $('#codigo').val(codigo);
   
    $('#direccion').val(direccion);

    
    $('#telefono').val(telefono);
    $('#monedas').val(moneda);
    $('#codigo').val(codigo);



    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + idcompania + ', tipo: 1}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativo').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#correlativo').val(serieelec);
        }
    });


    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + idcompania + ', tipo: 2}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoNota').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#correlativoNota').val(serieNota);
        }
    });

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + idcompania + ', tipo: 3}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoFacCopia').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#correlativoFacCopia').val(serieCopia);
        }
    });
    

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + idcompania + ', tipo: 4}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoAbono').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#correlativoAbono').val(serieAbono);   
        }
    });


    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + idcompania + ', tipo: 5}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoDebito').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#correlativoDebito').val(serieDebito);
        }
    });
   

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_regiones.asmx/ObtenerDatosPorID',
        data: '{id: ' + idcompania + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#region').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#region').val(region);
        }
    });


    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + idcompania + ', tipo: 10}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoNotaCopia').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#correlativoNotaCopia').val(serieNotaCopia);
        }
    });


    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + idcompania + ', tipo: 6}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoDebitoCopia').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#correlativoDebitoCopia').val(serieDebitoCopia);
        }
    });


    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + idcompania + ', tipo: 7}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoAbonoCopia').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#correlativoAbonoCopia').val(serieAbonoCopia);
        }
    });


    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + idcompania + ', tipo: 8}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativosCajaElectronicos').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#correlativosCajaElectronicos').val(serieCaja)
        }
    });


    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + idcompania + ', tipo: 9}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativosCajaElectronicosCopia').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#correlativosCajaElectronicosCopia').val(serieCajaCopia);
        }
    });


    //muestra la modal
    $('#MdNuevo').modal('toggle');
    //cambia los botones
    $('#bt-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#bt-guardar').removeClass('btn-success');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-info');
}


// funcion para cargar datos en el formulario
function eliminar(id) {
    limpiar();
    $('#id').val(id);
    //muestra la modal de confirmacion
    $('#MdDeshabilitar').modal('toggle');

    //cambia los botones
    $('#bt-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#bt-guardar').removeClass('btn-success');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-info');
}



//metodo utilizado para mostrar lista de datos 
function mostrarDatos() {
    limpiar();
    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_sucursales.asmx/ObtenerDatos',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            //limpia la tabla
            $('#tbod-datos').html(null);
            $('#tab-datos').dataTable().fnDeleteRow();
            $('#tab-datos').dataTable().fnUpdate();
            $('#tab-datos').dataTable().fnDestroy();
        },
        success: function (msg) {
            var tds = "";
            $('#tbod-datos').html(null);
            $.each(msg.d, function () {
                tds = "<tr class='odd'><td>" + this.codigo + "</td><td>" + this.compania + "</td><td>" + this.region + "</td><td>" + this.descripcion + "</td><td> " +
                    "<span onclick='cargarenFormulario(" + this.id + "," + this.codigo + ",\"" + this.descripcion + "\"," + this.idcompania + ", " + this.idmoneda + ", \"" + this.direccion + "\"," + this.idregion + "," + this.serieelec + "," + this.serieCopia + "," + this.serieNota + "," + this.serieDebito + "," + this.serieAbono + ",\"" + this.telefono + "\", " + this.serieNotaCopia + ", " + this.serieDebitoCopia + ", " + this.serieAbonoCopia +", "+ this.serieCaja +","+ this.serieCajaCopia +")' class='Mdnew btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' > " +
                    "<i class='material-icons'>edit</i> " +
                    "</span> " +
                    "<span onclick='eliminar(" + this.id + ")' class='btn btn-sm btn-outline-danger' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder Inhabilitar el dato seleccionado, Esto hara que dicho dato no aparesca en ninguna acción, menu o formulario del sistema.' data-original-title='' title=''> " +
                    "<i class='material-icons'> delete_sweep </i> " +
                    "</span></td></tr>'"
                $("#tbod-datos").append(tds);
            });

            $('#tab-datos').dataTable();
            $('[data-toggle="popover"]').popover();
        }
    });

};


//funcion para cargar las companias
function cargarCompanias() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_empresas.asmx/ObtenerDatos',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#company').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}

//funcion para cargar los tipos de fac
function CargarFacturacionElectronica(empresa) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: '+ empresa +', tipo: 1}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativo').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}


//funcion para cargar los tipos de fac
function CargarFacturacionNotaCreditoCopia(empresa) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + empresa + ', tipo: 10}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoNotaCopia').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}

//funcion para cargar los tipos de fac
function CargarFacturacionNotaCredito(empresa) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + empresa + ', tipo: 2}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoNota').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}

//funcion para cargar los tipos de fac
function CargarFacturacionCopia(empresa) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + empresa + ', tipo: 3}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoFacCopia').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}


//funcion para cargar los tipos de fac
function CargarFacturacionAbono(empresa) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + empresa + ', tipo: 4}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoAbono').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}


//funcion para cargar los tipos de fac
function CargarFacturacionAbonoCopio(empresa) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + empresa + ', tipo: 7}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoAbonoCopia').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}

//funcion para cargar los tipos de fac
function CargarFacturacionDebito(empresa) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + empresa + ', tipo: 5}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoDebito').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}

//funcion para cargar los tipos de fac
function CargarFacturacionDebitoCopia(empresa) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + empresa + ', tipo: 6}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativoDebitoCopia').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}


//funcion para cargar los tipos de fac
function CargarFacturacionCaja(empresa) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + empresa + ', tipo: 8}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativosCajaElectronicos').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}

//funcion para cargar los tipos de fac
function CargarFacturacionCajaCopia(empresa) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_correlativos.asmx/ObtenerDatosPorTipoEmpresa',
        data: '{id_empresa: ' + empresa + ', tipo: 9}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#correlativosCajaElectronicosCopia').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
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
                $('#monedas').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}


