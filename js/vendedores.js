$(function () {

    //llamada a metodos necesarios para el sistema
    $(function () {
        mostrarVendedores();


        "use strict"
        $(document).ready(function () {
            $('#dataTables-example').DataTable({
                "order": [
                [1, "desc"]
            ]
            });
        });

        $('.Mdnew').click(function () {
            $('#id').val(0);
            $('#MdNuevo').modal('toggle');
        });


        $('.MdDes').click(function () {
        });



        //accion  para guardar los datos
        $('#bt-guardar').click(function () {
            $('#bt-guardar').attr('disabled', true);
            $('#bt-cancelar').attr('disabled', true);
            var nombre = $('#nombre');
            var comisiones = $('#comision');
            var telefono = $('#telefono');
            var correo = $('#correo');
            var id = $('#id').val();


            if (nombre.val() != "" && comisiones.val() != 0 && telefono.val() != "" && correo.val() != "") {

                if (id == 0) {
                    insertar(nombre.val(), comisiones.val(), telefono.val(), correo.val());
                }
                else {
                    actualizar(nombre.val(), comisiones.val(), telefono.val(), correo.val(), id);
                }

                $('#nombre').removeClass('is-invalid');
                $('#nombre').removeClass('is-valid');

                $('#comision').removeClass('is-invalid');
                $('#comision').removeClass('is-valid');

                $('#telefono').removeClass('is-invalid');
                $('#telefono').removeClass('is-valid');

                $('#correo').removeClass('is-invalid');
                $('#correo').removeClass('is-valid');
            }
            else {

                if (nombre.val() == "") {
                    nombre.addClass('is-invalid');
                    nombre.focus();
                } else {
                    nombre.addClass('is-valid');
                }


                if (comisiones.val() == 0) {
                    comisiones.addClass('is-invalid');
                    comisiones.focus();
                } else {
                    comisiones.addClass('is-valid');
                }



                if (telefono.val() == "") {
                    telefono.addClass('is-invalid');
                    telefono.focus();
                } else {
                    telefono.addClass('is-valid');
                }


                if (correo.val() == "") {
                    correo.addClass('is-invalid');
                    correo.focus();
                } else {
                    correo.addClass('is-valid');
                }
            }
            /* -- METODOS DE INSERTAR -- */
        })



        //accion  para guardar los datos
        $('#bt-cancelar').click(function () {
            $('#nombre').val("");
            $('#comision').val(0);
            $('#telefono').val("");
            $('#correo').val("");

            $('#nombre').removeClass('is-invalid');
            $('#nombre').removeClass('is-valid');

            $('#telefono').removeClass('is-invalid');
            $('#telefono').removeClass('is-valid');

            $('#correo').removeClass('is-invalid');
            $('#correo').removeClass('is-valid');
        })


    });


    $('#btnDes').click(function () {
        var id = $('#ide').val();
        eliminar(id);
    });



});



// funcion para cargar datos en el formulario
function Editar(id, comision, nombre, telefono, correo) {
    $('#id').val(id);
    $('#nombre').val(nombre);
    $('#comision').val(comision);
    $('#telefono').val(telefono);
    $('#correo').val(correo);
    $('#bt-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#bt-guardar').removeClass('btn-success');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-info');
    $('#MdNuevo').modal('toggle');
}

function des(id) {
    $('#ide').val(id);
    $('#MdDeshabilitar').modal('toggle')
}

//metodo utilizado para insertar un vendedor
function insertar(nombre, comision, telefono, correo) {
    $.ajax({
        type: 'POST',
        url: 'wsvendedores.asmx/GuardarDatos',
        data: '{ nombre : "' + nombre + '",  comision : "' + comision + '",  telefono:"' + telefono + '", correo: "'+correo+'"}',
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
                $('.jq-toast-wrap').remove();
                document.getElementById('nombre').value = '';
                document.getElementById('comision').value = 0;
                document.getElementById('telefono').value = '';
                document.getElementById('correo').value = '';
                $('#bt-guardar').attr('disabled', false);
                $('#bt-cancelar').attr('disabled', false);
                $('#bt-guardar').removeClass('btn-warning');
                $('#bt-guardar').addClass('btn-success');
                $('#bt-guardar').html('<i class="material-icons">add</i> Guardar');
                $('#MdNuevo').modal('toggle');
                $.toast({
                    heading: 'EXITO!',
                    text: 'Datos ingresados exitosamente',
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'success',
                    stack: false
                })
                mostrarVendedores();
            }

        }
    });
}



//metodo utilizado para insertar un vendedor
function actualizar(nombre, comision, telefono, correo, id) {
    $.ajax({
        type: 'POST',
        url: 'wsvendedores.asmx/ActualizarDatos',
        data: '{ nombre : "' + nombre + '",  comision : "' + comision + '",  telefono:"' + telefono + '", correo: "' + correo + '", id: '+ id +'}',
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
                document.getElementById('comision').value = 0;
                document.getElementById('telefono').value = '';
                document.getElementById('correo').value = '';
                $('#bt-guardar').attr('disabled', false);
                $('#bt-cancelar').attr('disabled', false);
                $('#bt-guardar').removeClass('btn-warning');
                $('#bt-guardar').addClass('btn-success');
                $('#bt-guardar').html('<i class="material-icons">add</i> Guardar');
                $('#MdNuevo').modal('toggle');
                $.toast({
                    heading: 'Exito!',
                    text: 'Datos actualizados exitosamente',
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'success',
                    stack: false
                })
                mostrarVendedores();
            }

        }
    });
}

//metodo utilizado para insertar un vendedor
function eliminar(id) {
    $('#bt-eliminar').attr('disabled', true);

    $.ajax({
        type: 'POST',
        url: 'wsvendedores.asmx/Eliminar',
        data: '{id: ' + id + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $('#bt-eliminar').removeClass('btn-success');
            $('#bt-eliminar').addClass('btn-warning');
            $('#bt-eliminar').html('<i class="material-icons">query_builder</i>Cargando...')
        },
        success: function correcto(msg) {

            var confirma = msg.d;
            if (confirma == true) {
                document.getElementById('id').value = '';
                $('#btnDes').attr('disabled', false);
                $('#bt-eliminar').attr('disabled', false);
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
                mostrarVendedores();
            }

        }
    });
}

//metodo utilizado para mostrar lista de datos 
function mostrarVendedores() {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsvendedores.asmx/ObtenerDatos",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $('#tbod-datos').html(null);
            $('#tab-datos').dataTable().fnDeleteRow();
            $('#tab-datos').dataTable().fnUpdate();
            $('#tab-datos').dataTable().fnDestroy();
        },
        success: function (msg) {
            $.each(msg.d, function () {
                tds += '<tr class="odd">';
                tds = tds + '<td>' + this.id + '</td>';
                tds = tds + '<td>' + this.nombre + '</td>';
                tds = tds + '<td>' + this.telefono + '</td>';
                tds = tds + '<td>' + this.correo + '</td>';
                tds = tds + '<td class="center">';
                tds = tds + "<button class='btn btn-sm btn-outline-info' onclick='Editar(" + this.id + "," + this.comision + ",\"" + this.nombre + "\",\"" + this.telefono + "\",\"" + this.correo + "\")' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' >";
                tds = tds + '<i class="material-icons">edit</i> ';
                tds = tds + '</button>';
                tds = tds + "<button class='MdDes btn btn-sm btn-outline-danger' onclick='des(" + this.id + ")' data-toggle='tooltip' data-placement='bottom' title='Activo' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder Inhabilitar el dato seleccionado, Esto hara que dicho dato no aparesca en ninguna acción, menu o formulario del sistema.' data-original-title='' title=''><i class='material-icons'>delete_sweep</i></button>";
                tds = tds + '</td>';
                tds = tds + '</tr>';
            });


            $('[data-toggle="popover"]').popover();
            $('[data-toggle="tooltip"]').tooltip();
            $("#tbod-datos").append(tds);
            $('#tab-datos').dataTable();
        }

    });

}