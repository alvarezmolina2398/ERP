$(function () {

    //llamada a metodos necesarios para el sistema
    $(function () {
        mostrarMarcas();


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
            $('#inlineRadio1').attr('cheked', true);
            document.getElementById('desc').value = '';
        });


        $('.MdDes').click(function () {
        });



        //accion  para guardar los datos
        $('#bt-guardar').click(function () {
            var nombre = $('#desc');
            var id = $('#id').val();
            var adicional = $('input:radio[name=inlineRadioOptions]:checked').val();

            if (nombre.val() != "") {
                nombre.addClass('is-valid');

                if (id == 0) {
                    insertar(nombre.val(), adicional);
                }
                else {
                    actualizar(nombre.val(), adicional, id);
                }

                $('#desc').removeClass('is-invalid');
                $('#desc').removeClass('is-valid');

            }
            else {
                nombre.addClass('is-invalid');
                nombre.focus();
            }
            /* -- METODOS DE INSERTAR -- */
        });



        //accion  para guardar los datos
        $('#bt-cancelar').click(function () {
            $('#desc').val("");

            $('#desc').removeClass('is-invalid');
            $('#desc').removeClass('is-valid');
        })


    });


    $('#btnDes').click(function () {
        var id = $('#ide').val();
        eliminar(id);
    });



});



// funcion para cargar datos en el formulario
function Editar(id, adicional, nombre) {
    $('#id').val(id);
    $('#desc').val(nombre);
    $('#bt-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#bt-guardar').removeClass('btn-success');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-info');

    if (adicional == 1) {
        $('#inlineRadio1').attr('cheked', true);
    }
    else {
        $('#inlineRadio2').attr('cheked', true);
    }

    $('#MdNuevo').modal('toggle');
}

function des(id) {
    $('#ide').val(id);
    $('#MdDeshabilitar').modal('toggle')
}

//metodo utilizado para insertar un tipo de articulo
function insertar(nombre, adicional) {
    $.ajax({
        type: 'POST',
        url: 'wsformaspago.asmx/GuardarDatos',
        data: '{ nombre : "' + nombre + '",  adicional : ' + adicional +'}',
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
                document.getElementById('desc').value = '';
                $('#bt-guardar').attr('disabled', false);
                $('#MdNuevo').modal('toggle');
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
                mostrarMarcas();
            }

        }
    });
}



//metodo utilizado para actualizar un tipo de articulo
function actualizar(nombre, adicional, id) {
    $.ajax({
        type: 'POST',
        url: 'wsformaspago.asmx/ActualizarDatos',
        data: '{ nombre : "' + nombre + '", id: ' + id + ', adicional : '+ adicional +' }',
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
                document.getElementById('desc').value = '';
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
                mostrarMarcas();
            }

        }
    });
}

//metodo utilizado para eliminar un tipo de articulo
function eliminar(id) {
    $.ajax({
        type: 'POST',
        url: 'wsformaspago.asmx/Eliminar',
        data: '{id: ' + id + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $('#btnDes').removeClass('btn-success');
            $('#btnDes').addClass('btn-warning');
            $('#btnDes').html('<i class="material-icons">query_builder</i>Cargando...')
        },
        success: function correcto(msg) {

            var confirma = msg.d;
            if (confirma == true) {
                document.getElementById('id').value = '';
                $('#btnDes').attr('disabled', false);
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
                mostrarMarcas();
            }

        }
    });
}

//metodo utilizado para mostrar el listado de tipos de articulo 
function mostrarMarcas() {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsformaspago.asmx/ObtenerDatos",
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
                tds = tds + '<td class="center">';
                tds = tds + "<button class='btn btn-sm btn-outline-info' onclick='Editar(" + this.id + "," +this.adicional +",\"" + this.nombre + "\")' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' >";
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