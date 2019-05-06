$(function () {

    //oculta el panel
    $('#pn-principal').hide();

    cargarCompanias();
    mostrarDatos();

    $('#mdNew').click(function () {
        limpiar();
    });

    //accion  al momento de acepatar elminar
    $('#bt-eliminar').click(function () {
        $('#bt-eliminar').attr('disabled', true);
        $('#bt-no').attr('disabled', true);
        var id = $('#id').val()
        //consume el ws para obtener los datos
        $.ajax({
            url: 'wsadmin_bodegas.asmx/Inhabilitar',
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
    $('#company').change(function () {
        $('#region').html('<option value="0">Seleccione Una Opción</option>');

        if ($(this).val() != 0) {
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


            $.ajax({
                url: 'wsadmin_bodegas.asmx/ExistePrincipal',
                data: '{id: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    if (msg.d) {
                        //oculta el panel
                        $('#pn-principal').hide();
                    } else {
                        //muestra el panel
                        $('#principal').prop('checked', false);
                        $('#pn-principal').show();
                    }
                }
            });

        }
    });

    //accion para cargar las sucursales al cambiar de region
    $('#region').change(function () {
        $('#sucursal').html('<option value="0">Seleccione Una Opción</option>');

        if ($(this).val() != 0 && $('#company').val()) {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsadmin_sucursales.asmx/ObtenerDatosPorID',
                data: '{idregion: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        $('#sucursal').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
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
        var sucursal = $('#sucursal');
        var observacion = $('#observacion');
        var region = $('#region');
        var id = $('#id').val();
        var principal = 0;

        
        if ($('#principal').prop('checked')) {
            principal = 1;
        }


        if (validarForm()) {
            var data1 = '';
            var url1 = ''
            if (id != 0) {
                url1 = 'Actualizar'
                data1 = '{descripcion : "'+ descripcion.val() +'",  observacion : "'+ observacion.val() +'",  empresa : "'+ company.val()+'",  idsucursal : '+ sucursal.val()+',  id : '+id+', principal: '+ principal  +'}';
            } else {
                url1 = 'Insertar'
                data1 = '{descripcion : "' + descripcion.val() + '",  observacion : "' + observacion.val() + '",  empresa : "' + company.val() + '",  idsucursal : ' + sucursal.val() + ', principal: ' + principal +'}';
            }

            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsadmin_bodegas.asmx/' + url1,
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


function validarForm() {
    var company = $('#company');
    var descripcion = $('#descripcion');
    var sucursal = $('#sucursal');
    var observacion = $('#observacion');
    var region = $('#region');


    company.removeClass('is-invalid');
    company.removeClass('is-valid');

    descripcion.removeClass('is-invalid');
    descripcion.removeClass('is-valid');

    sucursal.removeClass('is-invalid');
    sucursal.removeClass('is-valid');

    observacion.removeClass('is-invalid');
    observacion.removeClass('is-valid');

    region.removeClass('is-invalid');
    region.removeClass('is-valid');

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

    //valdia que el select sucursal este vacio
    if (sucursal.val() == 0) {
        sucursal.addClass('is-invalid');
        sucursal.focus();
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
        sucursal.addClass('is-valid');
    }

    //valida el campo de region
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

    //valida las observaciones
    if (observacion.val() == 0) {
        observacion.addClass('is-invalid');
        observacion.focus();
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
        observacion.addClass('is-valid');
    }

    return result;

}

//metodo para limpiar el formulario
function limpiar() {
    $('#company').val(0);
    $('#descripcion').val(null);
    $('#sucursal').html('<option value="0">Seleccione Una Opción</option>');
    $('#observacion').val(null);
    $('#region').html('<option value="0">Seleccione Una Opción</option>');
    $('#id').val(0);
    $('#principal').prop('checked', false);
    $('#company').removeClass('is-invalid');
    $('#company').removeClass('is-valid');

    $('#descripcion').removeClass('is-invalid');
    $('#descripcion').removeClass('is-valid');

    $('#sucursal').removeClass('is-invalid');
    $('#sucursal').removeClass('is-valid');

    $('#observacion').removeClass('is-invalid');
    $('#observacion').removeClass('is-valid');

    $('#region').removeClass('is-invalid');
    $('#region').removeClass('is-valid');

    $('#bt-guardar').removeAttr('disabled', true);
    $('#bt-cancelar').removeAttr('disabled', true);
    $('#bt-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#bt-guardar').removeClass('btn-info');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-success');
}

// funcion para cargar datos en el formulario
function cargarenFormulario(id, descripcion, observacion, company, sucursal, region,principal) {
    limpiar();
    $('#id').val(id);
    $('#descripcion').val(descripcion);
    $('#company').val(company);
    $('#observacion').val(observacion);

    $.ajax({
        url: 'wsadmin_bodegas.asmx/ExistePrincipal',
        data: '{id: ' + company + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            if (msg.d) {
                //oculta el panel
                $('#pn-principal').show();
            } else {
                //muestra el panel
                $('#principal').prop('checked', false);
                $('#pn-principal').show();
            }
        }
    });



    if (principal == 1) {
        $('#principal').prop('checked', true);
    } else {
        $('#principal').prop('checked', false);       
    }
    $('#region').html('<option value="0">Seleccione Una Opción</option>');
    $('#sucursal').html('<option value="0">Seleccione Una Opción</option>')
    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_regiones.asmx/ObtenerDatosPorID',
        data: '{id: ' + company + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#region').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#region').val(region)
        }
    });


    $.ajax({
        url: 'wsadmin_sucursales.asmx/ObtenerDatosPorID',
        data: '{idregion: ' + region + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#sucursal').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#sucursal').val(sucursal);
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
        url: 'wsadmin_bodegas.asmx/ObtenerDatos',
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
                tds = "<tr class='odd'><td>" + i + "</td><td>" + this.empresa + "</td><td>" + this.region + "</td><td>" + this.sucursal + "</td><td>" + this.descripcion + "</td><td> " +
                    "<span onclick='cargarenFormulario(" + this.id + ",\"" + this.descripcion + "\",\"" + this.observacion + "\"," + this.idempresa + "," + this.idsucursal + "," + this.idregion + "," + this.prioridad +")' class='Mdnew btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' > " +
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
