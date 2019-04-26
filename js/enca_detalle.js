$(function () {

    //llamada a metodos necesarios para el sistema
    $(function () {
        getMarcas();
        mostrarColores();

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
            document.getElementById('nombre').value = '';
            document.getElementById('menui').value = 0;
        });


        $('.MdDes').click(function () {
        });



        //accion  para guardar los datos
        $('#bt-guardar').click(function () {
            var nombre = $('#nombre');
            var menui = $('#menui');
            var dir = $('#dir');
            var id = $('#id').val();

            if (nombre.val() != "" && menui.val() != 0 && dir.val() != "") {

                if (id == 0) {
                    insertar(nombre.val(), menui.val(), dir.val());
                }
                else {
                    actualizar(menui.val(), nombre.val(), dir.val(), id);
                }

                $('#nombre').removeClass('is-invalid');
                $('#nombre').removeClass('is-valid');
                $('#menui').removeClass('is-invalid');
                $('#menui').removeClass('is-valid');
                $('#dir').removeClass('is-invalid');
                $('#dir').removeClass('is-valid');

            }
            else {
                if (nombre.val() == "") {
                    nombre.addClass('is-invalid');
                    nombre.focus();
                }
                else {
                    alert(nombre.val());
                    nombre.addClass('is-valid');
                }

                if (menui.val() == 0) {

                    menui.addClass('is-invalid');
                    menui.focus();
                }
                else {

                    menui.addClass('is-valid');
                }

                if (dir.val() == "") {
                    dir.addClass('is-invalid');
                    dir.focus();
                }
                else {
                }
            }
            /* -- METODOS DE INSERTAR -- */
        })



        //accion  para guardar los datos
        $('#bt-cancelar').click(function () {
            $('#nombre').val("");
            $('#menui').val(0);
            $('#dir').val("");

            $('#nombre').removeClass('is-invalid');
            $('#nombre').removeClass('is-valid');

            $('#dir').removeClass('is-invalid');
            $('#dir').removeClass('is-valid');

            $('#menui').removeClass('is-invalid');
            $('#menui').removeClass('is-valid');

        })


    });


    $('#btnDes').click(function () {
        var id = $('#ide').val();
        eliminar(id);
    });



});



// funcion para cargar datos en el formulario
function Editar(id, marca, nombre, dir) {
    $('#id').val(id);
    $('#nombre').val(nombre);
    $('#menui').val(marca);
    $('#dir').val(dir);
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
function insertar(nombre, menu, dir) {
    $.ajax({
        type: 'POST',
        url: 'wsdetalle.asmx/GuardarDatos',
        data: '{menu:' + menu + ', nombre : "' + nombre + '", dir: "'+ dir +'"}',
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
                document.getElementById('menui').value = 0;
                document.getElementById('dir').value = '';
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
                mostrarColores();
            }

        }
    });
}



//metodo utilizado para insertar un vendedor
function actualizar(menu, nombre, dir, id) {
    $.ajax({
        type: 'POST',
        url: 'wsdetalle.asmx/ActualizarDatos',
        data: '{menu: ' + menu + ', nombre : "' + nombre + '", dir: "'+ dir +'", id: ' + id + ' }',
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
                document.getElementById('menui').value = 0;
                document.getElementById('dir').value = '';
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
                mostrarColores();
            }

        }
    });
}

//metodo utilizado para insertar un vendedor
function eliminar(id) {
    $.ajax({
        type: 'POST',
        url: 'wsdetalle.asmx/Eliminar',
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

//metodo utilizado para mostrar lista de datos 
function mostrarColores() {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsdetalle.asmx/ObeterDatos",
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
                tds = tds + '<td>' + this.menu + '</td>';
                tds = tds + '<td>' + this.descripcion + '</td>';
                tds = tds + '<td class="center">';
                tds = tds + "<button class='btn btn-sm btn-outline-info' onclick='Editar(" + this.id + "," + this.idmenu + ",\"" + this.descripcion + "\",\""+ this.dir +"\")' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' >";
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

//metodo utilizado para obtener el listado de marcas
function getMarcas() {
    var tds = "";
    $('#menui').html(null);
    $.ajax({
        type: "POST",
        url: "wsencabezado.asmx/ObtenerDatos",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            tds += '<option selected value="0">Selecione una opcion</option>';
            $.each(msg.d, function () {
                tds += '<option  value="' + this.id + '">' + this.nombre + '</option>';

            });

            $('#menui').append(tds);
        }
    });

}