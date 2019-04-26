$(function () {
    $(document).ready(function () {
        ListaClasificacion();
    });

    //cambia la accion de la modal para insertar un departamento
    $('.Mdnew').click(function () {
        $('#id').val(0);
        $('#MdNuevo').modal('toggle');
    });

    //metodo utilizado para actualiar o insertar una clasificacion
    $('#bt-guardar').click(function () {
        var nombre = $('#nombre').val();
        var descripcion = $('#descripcion').val();
        var observacion = $('#observacion').val();
        var id = $('#id').val();

        if (id == 0) {
            insertar(nombre, descripcion, observacion);
        }
        else {
            actualizar(nombre, descripcion, observacion, id);
        }
    });

    $('#btnDes').click(function () {
        $.ajax({
            type: "POST",
            url: "wsclasificacionArt.asmx/cambiarEstado",
            data: '{ id: ' + $('#ide').val() + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                $('#btnDes').text("Cargando...");
                $('#btnDes').attr('disabled', true);
            },
            success: function correcto(msg) {

                var confirma = msg.d;
                if (confirma == true) {
                    $('#btnDes').attr('disabled', false);
                    $('#btnDes').text("si");
                    $('#MdDeshabilitar').modal('toggle');
                    ListaClasificacion();
                    $.toast({
                        heading: 'Listo!',
                        text: 'Se ha eliminado la clasificacion exitosamente',
                        showHideTransition: 'slide',
                        position: 'mid-center',
                        icon: 'success'
                    })
                }

            }
        });
    });

    $('#btnDes').click(function () {
        var id = $('#ide').val();
        eliminar(id);
    });


});


function des(id) {
    $('#ide').val(id);
    $('#MdDeshabilitar').modal('toggle')
}

// funcion para cargar datos en el formulario
function mostrar(id, nombre, descripcion, observacion) {
    $('#id').val(id);
    $('#nombre').val(nombre);
    $('#descripcion').val(descripcion);
    $('#observacion').val(observacion);
    $('#bt-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#bt-guardar').removeClass('btn-success');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-info');
    $('#MdNuevo').modal('toggle');
}

//metodo utilizado para actualizar una clasificacion
function actualizar(nombre, descripcion, observacion, id) {
    $.ajax({
        type: "POST",
        url: "wsclasificacionArt.asmx/actualizar",
        data: '{nombre: "' + nombre + '", descripcion: "' + descripcion + '", observacion: "' + observacion + '", id:' + id + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $('#bt-guardar').removeClass('btn-info');
            $('#bt-guardar').removeClass('btn-success');
            $('#bt-guardar').addClass('btn-warning');
            $('#bt-guardar').html('<i class="material-icons">query_builder</i>Cargando...')   
        },
        success: function correcto(msg) {
            var confirma = msg.d;
            if (confirma == true) {
                document.getElementById('nombre').value = '';
                document.getElementById('descripcion').value = '';
                document.getElementById('observacion').value = '';
                $('#bt-guardar').attr('disabled', false);
                $('#bt-guardar').removeClass('btn-warning');
                $('#bt-guardar').addClass('btn-success');
                $('#bt-guardar').html('<i class="material-icons">add</i> Guardar');
                $('#MdNuevo').modal('toggle');
                $.toast({
                    heading: 'Listo!',
                    text: 'Datos actualizados exitosamente',
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'success',
                    stack: false
                })
                ListaClasificacion();
            }

        }
    });
}

//metodo utilizado para insertar una clasificacion
function insertar(nombre, descripcion, observacion) {
    if (nombre == "") {
        $.toast({
            heading: 'Error',
            text: 'debe de ingresar un nombre',
            showHideTransition: 'fade',
            position: 'mid-center',
            icon: 'error'
        });
    }
    else if (descripcion == "") {
        $.toast({
            heading: 'Error',
            text: 'debe de ingresar una descripcion',
            showHideTransition: 'fade',
            position: 'mid-center',
            icon: 'error'
        });
    }
    else if (observacion == "") {
        $.toast({
            heading: 'Error',
            text: 'debe de ingresar observacion',
            showHideTransition: 'fade',
            position: 'mid-center',
            icon: 'error'
        });
    }
    else {
        $.ajax({
            type: "POST",
            url: "wsclasificacionArt.asmx/insertar",
            data: '{nombre: "' + nombre + '", descripcion: "' + descripcion + '", observacion: "' + observacion + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                $('#bt-guardar').removeClass('btn-info');
                $('#bt-guardar').removeClass('btn-success');
                $('#bt-guardar').addClass('btn-warning');
                $('#bt-guardar').html('<i class="material-icons">query_builder</i>Cargando...')   
            },
            success: function correcto(msg) {
                var confirma = msg.d;
                if (confirma == true) {
                    document.getElementById('nombre').value = '';
                    document.getElementById('descripcion').value = '';
                    document.getElementById('observacion').value = '';
                    $('#bt-guardar').attr('disabled', false);
                    $('#bt-guardar').removeClass('btn-warning');
                    $('#bt-guardar').addClass('btn-success');
                    $('#bt-guardar').html('<i class="material-icons">add</i> Guardar');
                    $('#MdNuevo').modal('toggle');
                    $.toast({
                        heading: 'Listo!',
                        text: 'Clasificacion creada exitosamente',
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'success',
                        stack: false
                    })
                    ListaClasificacion();
                }

            }
        });
    }
}

//metodo utilizado para insertar un vendedor
function eliminar(id) {
    $.ajax({
        type: 'POST',
        url: 'wsclasificacionArt.asmx/cambiarEstado',
        data: '{id: ' + id + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $('#btnDes').removeClass('btn-success');
            $('#btnDes').addClass('btn-warning');
            $('#btnDes').html('<i class="material-icons">query_builder</i>Cargando...');
        },
        success: function correcto(msg) {

            var confirma = msg.d;
            if (confirma == true) {
                document.getElementById('id').value = '';
                $('#btnDes').attr('disabled', false);
                $('#MdDeshabilitar').modal('toggle');
                $('#btnDes').removeClass('btn-warning');
                $('#btnDes').addClass('btn-success');
                $('#btnDes').text("Si");
                $('#MdDeshabilitar').modal('toggle');
                $('.jq-toast-wrap').remove();
                $.toast({
                    heading: 'Listo!',
                    text: 'Datos Eliminados exitosamente',
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'success',
                    stack: false
                })
                mostrarColores();
            }

        }
    });
}

//metodo utilizado para obtener la lista de clasificacion de articulos
function ListaClasificacion() {
    $.ajax({
        type: 'POST',
        url: 'wsclasificacionArt.asmx/ObtenerClasificacion',
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $('#tbod-datos').empty();
            $('#tab-datos').dataTable().fnDeleteRow();
            $('#tab-datos').dataTable().fnUpdate();
            $('#tab-datos').dataTable().fnDestroy();
        },
        success: function (msg) {
            var tds = "";
            $.each(msg.d, function () {
                tds += '<tr class="odd">';
                tds = tds + '<td>' + this.id_clasif + '</td>';
                tds = tds + '<td>' + this.id_nombre + '</td>';
                tds = tds + '<td>' + this.id_descripcion + '</td>';
                tds = tds + '<td>' + this.id_observacion + '</td>';
                tds = tds + '<td class="center">';
                tds = tds + "<button class='Mdnew btn btn-sm btn-outline-info' onclick='mostrar(" + this.id_clasif + ",\"" + this.id_nombre + "\",\"" + this.id_descripcion + "\",\"" + this.id_observacion + "\")' data-toggle='tooltip' data-placement='bottom' title='Editar'>";
                tds = tds + '<i class="material-icons">edit</i> ';
                tds = tds + '</button>';
                tds = tds + "<button class='MdDes btn btn-sm btn-outline-danger' onclick='des(" + this.id_clasif + ")' data-toggle='tooltip' data-placement='bottom' title='Activo' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder Inhabilitar el dato seleccionado, Esto hara que dicho dato no aparesca en ninguna acción, menu o formulario del sistema.' data-original-title='' title=''><i class='material-icons'>delete_sweep</i></button>";
                tds = tds + '</td>';
                tds = tds + '</tr>';
            });

            $("#tbod-datos").append(tds);
            $('#tab-datos').dataTable();
        }

    });

}