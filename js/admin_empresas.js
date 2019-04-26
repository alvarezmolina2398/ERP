$(function () {
    mostrarDatos();
    cargarDepartamentos();
    cargarActividades();

    $('#mdNew').click(function () {
        limpiar();
    });

    $('input[name="fecha"]').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        locale: {
            format: 'DD/MM/YYYY'
        },
        minYear: 1901
    }, function (start, end, label) { });

    //accion  al momento de acepatar elminar
    $('#bt-eliminar').click(function () {
        $('#bt-eliminar').attr('disabled', true);
        $('#bt-no').attr('disabled', true);
        var id = $('#id').val()
        //consume el ws para obtener los datos
        $.ajax({
            url: 'wsadmin_empresas.asmx/Inhabilitar',
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

    //accion para cargar las regiones al cambiar de agencia
    $('#departamento').change(function () {
        $('#municipio').html('<option value="0">Seleccione Una Opción</option>');

        if ($(this).val() != 0) {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wscargar_datos.asmx/cargarMunicipiosPorDep',
                data: '{dep: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        $('#municipio').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                    });
                }
            });

        }
    });


    //accion  para guardar o actualizar los datos
    $('#bt-guardar').click(function () {
        $('#bt-guardar').attr('disabled', true);
        $('#bt-cancelar').attr('disabled', true);
        var nit = $('#nit');
        var nombre = $('#nombre');
        var nombrecomercial = $('#razon');
        var telefono = $('#telefono');
        var direccion = $('#direccion');
        var id = $('#id').val();
        var departamento = $('#departamento');
        var municipio = $('#municipio');
        var actividad = $('#actividad');
        var tipoPago = $('#tipoPago');
        var numeroest = $('#numeroest');
        var numeroreg = $('#numeroreg');
        var fechaRegistro = $('#fechaRegistro');
        var fechaConstitucion = $('#fechaConstitucion');



        if (validarForm()) {
            var data1 = '';
            var url1 = ''
            if (id != 0) {
                url1 = 'Actualizar'
                data1 = '{ nombre : "' + nombre.val() + '",  nombrecomercial : "' + nombrecomercial.val() + '",  dep : ' + departamento.val() + ',  mun : ' + municipio.val() + ',  direccion : "' + direccion.val() + '",  telefono : "' + telefono.val() + '",  actividad : ' + actividad.val() + ',  tipoPago : ' + tipoPago.val() + ',  nit : "' + nit.val() + '",  noesta : ' + numeroest.val() + ',  noregistro : ' + numeroreg.val() + ',  fechaCons : "' + fechaConstitucion.val() + '",  fecharegistro : "' + fechaRegistro.val() + '", id : '+ id +'}';
            } else {
                url1 = 'Insertar'
                data1 = '{ nombre : "'+ nombre.val() +  '",  nombrecomercial : "' + nombrecomercial.val() + '",  dep : ' + departamento.val() + ',  mun : ' + municipio.val() + ',  direccion : "'+direccion.val()+'",  telefono : "'+telefono.val()+'",  actividad : '+actividad.val()+',  tipoPago : '+ tipoPago.val()+',  nit : "'+nit.val()+'",  noesta : '+ numeroest.val()+',  noregistro : '+numeroreg.val()+',  fechaCons : "'+fechaConstitucion.val()+'",  fecharegistro : "'+fechaRegistro.val()+'"}';
            }

            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsadmin_empresas.asmx/' + url1,
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


    //accion  para cancelar los datos
    $('#bt-cancelar').click(function () {
        limpiar();
    })

});



//funcion para cargar las Departamentos
function cargarDepartamentos() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarDepartamentos',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#departamento').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}

//funcion para cargar las Departamentos
function cargarActividades() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarActividades',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#actividad').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}


function validarForm() {
    var nit = $('#nit');
    var nombre = $('#nombre');
    var nombrecomercial = $('#razon');
    var telefono = $('#telefono');
    var direccion = $('#direccion');
    var departamento = $('#departamento');
    var municipio = $('#municipio');
    var actividad = $('#actividad');
    var tipoPago = $('#tipoPago');
    var numeroest = $('#numeroest');
    var numeroreg = $('#numeroreg');
    var fechaRegistro = $('#fechaRegistro');
    var fechaConstitucion = $('#fechaConstitucion');


    nit.removeClass('is-invalid');
    nit.removeClass('is-valid');

    nombre.removeClass('is-invalid');
    nombre.removeClass('is-valid');

    nombrecomercial.removeClass('is-invalid');
    nombrecomercial.removeClass('is-valid');

    telefono.removeClass('is-invalid');
    telefono.removeClass('is-valid');

    direccion.removeClass('is-invalid');
    direccion.removeClass('is-valid');

    departamento.removeClass('is-invalid');
    departamento.removeClass('is-valid');

    municipio.removeClass('is-invalid');
    municipio.removeClass('is-valid');

    actividad.removeClass('is-invalid');
    actividad.removeClass('is-valid');

    tipoPago.removeClass('is-invalid');
    tipoPago.removeClass('is-valid');

    numeroest.removeClass('is-invalid');
    numeroest.removeClass('is-valid');

    numeroreg.removeClass('is-invalid');
    numeroreg.removeClass('is-valid');

    fechaRegistro.removeClass('is-invalid');
    fechaRegistro.removeClass('is-valid');

    fechaConstitucion.removeClass('is-invalid');
    fechaConstitucion.removeClass('is-valid');

    var result = true
    if (nit.val() == "") {
        nit.addClass('is-invalid');
        nit.focus();
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
        nit.addClass('is-valid');
    }

    if (nombre.val() == "") {
        nombre.addClass('is-invalid');
        nombre.focus();
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
        nombre.addClass('is-valid');
    }

    if (nombrecomercial.val() == "") {
        nombrecomercial.addClass('is-invalid');
        nombrecomercial.focus();
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
        nombrecomercial.addClass('is-valid');
    }

    if (nombre.val() == "") {
        nombre.addClass('is-invalid');
        nombre.focus();
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
        nombre.addClass('is-valid');
    }

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

    if (departamento.val() == 0) {
        departamento.addClass('is-invalid');
        departamento.focus();
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
        departamento.addClass('is-valid');
    }


    if (municipio.val() == 0) {
        municipio.addClass('is-invalid');
        municipio.focus();
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
        municipio.addClass('is-valid');
    }


    if (actividad.val() == 0) {
        actividad.addClass('is-invalid');
        actividad.focus();
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
        actividad.addClass('is-valid');
    }


    if (tipoPago.val() == 0) {
        tipoPago.addClass('is-invalid');
        tipoPago.focus();
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
        tipoPago.addClass('is-valid');
    }
    

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

    if (numeroest.val() == "") {
        numeroest.addClass('is-invalid');
        numeroest.focus();
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
        numeroest.addClass('is-valid');
    }

    if (numeroreg.val() == "") {
        numeroreg.addClass('is-invalid');
        numeroreg.focus();
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
        numeroreg.addClass('is-valid');
    }


    if (fechaRegistro.val() == "") {
        fechaRegistro.addClass('is-invalid');
        fechaRegistro.focus();
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
        fechaRegistro.addClass('is-valid');
    }

    if (fechaConstitucion.val() == "") {
        fechaConstitucion.addClass('is-invalid');
        fechaConstitucion.focus();
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
        fechaConstitucion.addClass('is-valid');
    }


    return result;

}

//metodo para limpiar el formulario
function limpiar() {

    $('#nit').val(null);
    $('#nombre').val(null);
    $('#razon').val(null);
    $('#telefono').val(null);
    $('#direccion').val(null);
    $('#id').val(0);
    $('#departamento').val(0);
    $('#municipio').html('<option value="0">Seleccione Una Opción</option>');
    $('#actividad').val(0);
    $('#tipoPago').val(0);
    $('#numeroest').val(null);
    $('#numeroreg').val(null);
    $('#fechaRegistro').val(null);
    $('#fechaConstitucion').val(null);


    var nit = $('#nit');
    var nombre = $('#nombre');
    var nombrecomercial = $('#razon');
    var telefono = $('#telefono');
    var direccion = $('#direccion');
    var departamento = $('#departamento');
    var municipio = $('#municipio');
    var actividad = $('#actividad');
    var tipoPago = $('#tipoPago');
    var numeroest = $('#numeroest');
    var numeroreg = $('#numeroreg');
    var fechaRegistro = $('#fechaRegistro');
    var fechaConstitucion = $('#fechaConstitucion');


    nit.removeClass('is-invalid');
    nit.removeClass('is-valid');

    nombre.removeClass('is-invalid');
    nombre.removeClass('is-valid');

    nombrecomercial.removeClass('is-invalid');
    nombrecomercial.removeClass('is-valid');

    telefono.removeClass('is-invalid');
    telefono.removeClass('is-valid');

    direccion.removeClass('is-invalid');
    direccion.removeClass('is-valid');

    departamento.removeClass('is-invalid');
    departamento.removeClass('is-valid');

    municipio.removeClass('is-invalid');
    municipio.removeClass('is-valid');

    actividad.removeClass('is-invalid');
    actividad.removeClass('is-valid');

    tipoPago.removeClass('is-invalid');
    tipoPago.removeClass('is-valid');

    numeroest.removeClass('is-invalid');
    numeroest.removeClass('is-valid');

    numeroreg.removeClass('is-invalid');
    numeroreg.removeClass('is-valid');

    fechaRegistro.removeClass('is-invalid');
    fechaRegistro.removeClass('is-valid');

    fechaConstitucion.removeClass('is-invalid');
    fechaConstitucion.removeClass('is-valid');


    $('#bt-guardar').removeAttr('disabled', true);
    $('#bt-cancelar').removeAttr('disabled', true);
    $('#bt-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#bt-guardar').removeClass('btn-info');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-success');
}

// funcion para cargar datos en el formulario
function cargarenFormulario(id, descripcion, nombrecomercial, nit, telefono, direccion, departamento, municipio, actividad, tipoPago, numeroest, numeroreg, fechaRegistro, fechaConstitucion) {
    limpiar();



    $('#nit').val(nit);
    $('#nombre').val(descripcion);
    $('#razon').val(nombrecomercial);
    $('#telefono').val(telefono);
    $('#direccion').val(direccion);
    $('#id').val(id);
    $('#departamento').val(departamento);
    $('#municipio').html('<option value="0">Seleccione Una Opción</option>');
    $('#actividad').val(actividad);
    $('#tipoPago').val(tipoPago);
    $('#numeroest').val(numeroest);
    $('#numeroreg').val(numeroreg);
    $('#fechaRegistro').val(fechaRegistro);
    $('#fechaConstitucion').val(fechaConstitucion);




    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarMunicipiosPorDep',
        data: '{dep: ' + departamento + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#municipio').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });

            $('#municipio').val(municipio)
        }
    });




    $('#MdNuevo').modal('toggle')

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
        url: 'wsadmin_empresas.asmx/ObtenerDatos',
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

                tds = "<tr class='odd'><td>" + i + "</td><td>" + this.nit + "</td><td>" + this.descripcion + "</td><td>" + this.nombrecomercial + "</td><td>" + this.telefono + "</td><td> " +
                    "<span onclick='cargarenFormulario(" + this.id + ",\"" + this.descripcion + "\",\"" + this.nombrecomercial + "\",\"" + this.nit + "\",\"" + this.telefono + "\",\"" + this.direccion + "\"," + this.iddep + "," + this.idmun + "," + this.actividad + "," + this.tipopago + "," + this.noesta + "," + this.noregistro + ",\"" + this.fecharegistro + "\",\"" + this.fechaconstitucion + "\")' class='Mdnew btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' > " +
                    "<i class='material-icons'>edit</i> " +
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
