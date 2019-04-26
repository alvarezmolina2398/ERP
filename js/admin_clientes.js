$(function () {
    cargarCompanias();
    mostrarDatos();
    cargarDepartamentos();
    cargarClasificacion();

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
            url: 'wsadmin_clientes.asmx/Inhabilitar',
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
        var descripcion = $('#descripcion');
        var telefono = $('#telefono');
        var observacion = $('#observacion');
        var direccion = $('#direccion');
        var id = $('#id').val();
        var departamento = $('#departamento');
        var municipio = $('#municipio');
        var clasificacion = $('#clasificacion');
        var contacto1 = $('#contacto1');
        var contacto2 = $('#contacto2');
        var contacto3 = $('#contacto3');
        var descuento = $('#descuento');
        var limite = $('#limite');
        var dia = $('#dias');
        var correo = $('#correo');
        if (validarForm()) {
            var data1 = '';
            var url1 = ''
            if (id != 0) {
                url1 = 'Actualizar'
                data1 = '{ nit_clt : "' + nit.val() + '",  Nom_clt : "' + descripcion.val() + '",  Tel_Clt : "' + telefono.val() + '",  Dire_Clt : "' + direccion.val() + '",  id_clasif : ' + clasificacion.val() + ',  id_dep : ' + departamento.val() + ',  id_muni : ' + municipio.val() + ',  contacto1 : "' + contacto1.val() + '",  contacto2 : "' + contacto2.val() + '",  contacto3 : "' + contacto3.val() + '",  Descuento_Porc : ' + descuento.val() + ',  Limite_Credito : ' + limite.val() + ',  Dias_Credito : ' + dia.val() + ',  Correo_Clt : "' + correo.val() + '",  Observ_Clt : "' + observacion.val() + '", id: '+id+' }';
            } else {
                url1 = 'Insertar'
                data1 = '{ nit_clt : "' + nit.val() + '",  Nom_clt : "' + descripcion.val() + '",  Tel_Clt : "' + telefono.val() + '",  Dire_Clt : "' + direccion.val() + '",  id_clasif : ' + clasificacion.val() + ',  id_dep : ' + departamento.val() + ',  id_muni : ' + municipio.val() + ',  contacto1 : "' + contacto1.val() + '",  contacto2 : "' + contacto2.val() + '",  contacto3 : "' + contacto3.val() + '",  Descuento_Porc : ' + descuento.val() + ',  Limite_Credito : ' + limite.val() + ',  Dias_Credito : ' + dia.val() + ',  Correo_Clt : "' + correo.val() + '",  Observ_Clt : "' + observacion.val() + '"}';
            }

            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsadmin_clientes.asmx/' + url1,
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
function cargarClasificacion() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarClasificacionCliente',
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


function validarForm() {
    var descripcion = $('#descripcion');
    var nit = $('#nit');
    var direccion = $('#direccion');
    var departamento = $('#departamento');
    var municipio = $('#municipio');

    descripcion.removeClass('is-invalid');
    descripcion.removeClass('is-valid');

    nit.removeClass('is-invalid');
    nit.removeClass('is-valid');

    direccion.removeClass('is-invalid');
    direccion.removeClass('is-valid');

    departamento.removeClass('is-invalid');
    departamento.removeClass('is-valid');

    municipio.removeClass('is-invalid');
    municipio.removeClass('is-valid');

    var descuento = $('#descuento');
    var limite = $('#limite');
    var dias = $('#dias');

    if (descuento.val() == "") {
        descuento.val(0);
    }

    if (limite.val() == "") {
        limite.val(0);
    }

    if (dias.val() == "") {
        dias.val(0);
    }

    var result = true
    if (descripcion.val() == "") {
        descripcion.addClass('is-invalid');
        descripcion.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Debe Ingresar el nombre del Cliente',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });
        result = false
    } else {
        descripcion.addClass('is-valid');
    }

     if (nit.val() == "") {
        nit.addClass('is-invalid');
        nit.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Debe Ingresar el Nit del Cliente',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });
         result = false
     } else {
         nit.addClass('is-valid');
     }
    if (direccion.val() == "") {
        direccion.addClass('is-invalid');
        direccion.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Debe Ingresar la Direccion del Cliente',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });
        result = false
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
            text: 'Debe seleccionar el departamento del Cliente',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });
        result = false
    } else {
        departamento.addClass('is-valid');
    }

    if (municipio.val() == 0 || municipio.val() == "" || municipio.val()== null) {
        municipio.addClass('is-invalid');
        municipio.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Debe seleccionar el municipio del Cliente',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });
         result = false
     } else {
         municipio.addClass('is-valid');
     }

    return result;

}

//metodo para limpiar el formulario
function limpiar() {

    $('#nit').val(null);
    $('#descripcion').val(null);
    $('#telefono').val(null);
    $('#observacion').val(null);
    $('#direccion').val(null);
    $('#id').val(0);
    $('#departamento').val(0);
    $('#municipio').html('<option value="0">Seleccione Una Opción</option>');
    $('#clasificacion').val(0);
    $('#contacto1').val(null);
    $('#contacto2').val(null);
    $('#contacto3').val(null);
    $('#descuento').val(0);
    $('#limite').val(0);
    $('#dias').val(0);
    $('#correo').val(null);


    var descripcion = $('#descripcion');
    var nit = $('#nit');
    var direccion = $('#direccion');
    var departamento = $('#departamento');
    var municipio = $('#municipio');

    descripcion.removeClass('is-invalid');
    descripcion.removeClass('is-valid');

    nit.removeClass('is-invalid');
    nit.removeClass('is-valid');

    direccion.removeClass('is-invalid');
    direccion.removeClass('is-valid');

    departamento.removeClass('is-invalid');
    departamento.removeClass('is-valid');

    municipio.removeClass('is-invalid');
    municipio.removeClass('is-valid');


    $('#bt-guardar').removeAttr('disabled', true);
    $('#bt-cancelar').removeAttr('disabled', true);
    $('#bt-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#bt-guardar').removeClass('btn-info');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-success');
}

// funcion para cargar datos en el formulario
function cargarenFormulario(id, descripcion, observacion, nit, telefono, direccion, departamento, municipio, clasificacion, contacto1, contacto2, contacto3,descuento,limite,dias,correo) {
    limpiar();


    $('#nit').val(nit);
    $('#descripcion').val(descripcion);
    $('#telefono').val(telefono);
    $('#observacion').val(observacion);
    $('#direccion').val(direccion);
    $('#id').val(id);
    $('#departamento').val(departamento);
    $('#municipio').html('<option value="0">Seleccione Una Opción</option>');
    $('#clasificacion').val(clasificacion);
    $('#contacto1').val(contacto1);
    $('#contacto2').val(contacto2);
    $('#contacto3').val(contacto3);
    $('#descuento').val(descuento);
    $('#limite').val(limite);
    $('#dias').val(dias);
    $('#correo').val(correo);



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
        url: 'wsadmin_clientes.asmx/ObtenerDatos',
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
                tds = "<tr class='odd'><td>" + i + "</td><td>" + this.nit + "</td><td>" + this.nombre + "</td><td>" + this.Telefono + "</td><td>" + this.contacto1 + "</td><td> " +
                    "<span onclick='cargarenFormulario(" + this.id + ",\"" + this.nombre + "\",\"" + this.observaciones + "\",\"" + this.nit + "\",\"" + this.Telefono + "\",\"" + this.Direccion + "\"," + this.id_dep + "," + this.id_mun + "," + this.id_clasificacion + ",\"" + this.contacto1 + "\",\"" + this.contacto2 + "\",\"" + this.contacto3 + "\"," + this.descuento + "," + this.limite + "," + this.dias + ",\"" + this.correo + "\")' class='Mdnew btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' > " +
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
