$(function () {

    //llamada a metodos necesarios para el sistema
    $(function () {
        mostrarUsuarios();
        obtenerDeptos();
        obtenerRoles();

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
            $('#bt-guardar').removeClass('btn-warning');
            $('#bt-guardar').addClass('btn-success');
            $('#bt-guardar').html('<i class="material-icons">add</i> Guardar');
            $('#MdNuevo').modal('toggle');
            limpiarForm();
        });


        $('.MdDes').click(function () {
        });



        //accion  para guardar los datos
        $('#bt-guardar').click(function () {
            var nombre = $('#nombre');
            var usuario = $('#usuario');
            var apellidos = $('#apellidos');
            var password = $('#password');
            var confirm = $('#confirm');
            var empresa = $('#emp');
            var sucursal = $('#sucursal');
            var tipo = $('#tipoU');
            var rol = $('#rol');
            var depto = $('#depto');
            var correo = $('#correo');
            var id = $('#id').val();


            if (usuario.val() != "" && nombre.val() != "" && apellidos.val() != "" && password.val() != ""
                && confirm.val() != "" && empresa.val() != 0 && sucursal.val() != 0 &&
                tipo.val() != 0 && rol.val() != 0 && depto.val() != 0 && correo != "") {

                if (id == 0) {
                    insertar(usuario.val(), nombre.val(), apellidos.val(), password.val(), empresa.val(), sucursal.val(), tipo.val(), rol.val(), depto.val(), correo.val());
                }
                else {
                    actualizar(usuario.val(), nombre.val(), apellidos.val(), password.val(), empresa.val(), sucursal.val(), tipo.val(), rol.val(), depto.val(), id, correo.val());
                }

                $('#nombre').removeClass('is-invalid');
                $('#nombre').removeClass('is-valid');
                $('#apellidos').removeClass('is-invalid');
                $('#apellidos').removeClass('is-valid');
                $('#password').removeClass('is-invalid');
                $('#password').removeClass('is-valid');
                $('#confirm').removeClass('is-invalid');
                $('#confirm').removeClass('is-valid');
                $('#empresa').removeClass('is-invalid');
                $('#empresa').removeClass('is-valid');
                $('#sucursal').removeClass('is-invalid');
                $('#sucursal').removeClass('is-valid');
                $('#correo').removeClass('is-invalid');
                $('#correo').removeClass('is-valid');
            }
            else {
                if (usuario.val() == "") {
                    usuario.addClass('is-invalid');
                    usuario.focus();
                } else {
                    usuario.addClass('is-valid');
                }

                if (nombre.val() == "") {
                    nombre.addClass('is-invalid');
                    nombre.focus();
                } else {
                    nombre.addClass('is-valid');
                }


                if (password.val() == "") {
                    password.addClass('is-invalid');
                    password.focus();
                } else {
                    password.addClass('is-valid');
                }

                if (empresa.val() == 0) {
                    empresa.addClass('is-invalid');
                    empresa.focus();
                } else {
                    empresa.addClass('is-valid');
                }

                if (sucursal.val() == 0) {
                    sucursal.addClass('is-invalid');
                    sucursal.focus();
                } else {
                    sucursal.addClass('is-valid');
                }


                if (tipo.val() == '0') {
                    tipo.addClass('is-invalid');
                    tipo.focus();
                } else {
                    tipo.addClass('is-valid');
                }

                if (rol.val() == 0) {
                    rol.addClass('is-invalid');
                    rol.focus();
                } else {
                    rol.addClass('is-valid');
                }

                if (depto.val() == 0) {
                    depto.addClass('is-invalid');
                    depto.focus();
                } else {
                    depto.addClass('is-valid');
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


    $('#emp').change(function () {
        obtenerSuc($(this).val());
    });


});



// funcion para cargar datos en el formulario
function Editar(id, empresa, sucursal, rol, depto, nombre, apellidos, password, usuario, tipo, correo) {
    filtrarSuc(sucursal,empresa);
    $('#id').val(id);
    $('#nombre').val(nombre);
    $('#apellidos').val(apellidos);
    $('#password').val(password);
    $('#confirm').val(password);
    $('#emp').val(empresa);
    $('#tipoU').val(tipo);
    $('#usuario').val(usuario);
    $('#rol').val(rol);
    $('#depto').val(depto);
    $('#sucursal').val(sucursal);
    $('#correo').val(correo);
    $('#bt-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#bt-guardar').removeClass('btn-success');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-info');
    $('#MdNuevo').modal('toggle');
}

$('#confirm').blur(function () {
    if ($(this).val() != $('#password').val()) {
        $.toast({
            heading: 'Error',
            text: 'la contrasena no coincide, Vuelva a intentarlo',
            showHideTransition: 'fade',
            position: 'mid-center',
            icon: 'error'
        });
    }
});

function des(id) {
    $('#ide').val(id);
    $('#MdDeshabilitar').modal('toggle')
}

//metodo utilizado para insertar un vendedor
function insertar(usuario,nombre, apellidos, password, empresa, sucursal, tipo, rol, depto, correo) {
    $.ajax({
        type: 'POST',
        url: 'wsusuarios.asmx/GuardarDatos',
        data: '{usuario: "'+usuario+'", nombre : "' + nombre + '",  apellidos : "' + apellidos + '",  password:"' + password + '", empresa: ' + empresa  + ', sucursal: '+sucursal+', tipo: "'+tipo+'", depto: '+depto+', rol: '+rol+', correo: "'+ correo +'"}',
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
                $('#bt-guardar').attr('disabled', false);
                $('#bt-guardar').text("Guardar");
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
                mostrarUsuarios();
                limpiarForm();
            }

        }
    });
}



//metodo utilizado para insertar un vendedor
function actualizar(usuario,nombre, apellidos, password, empresa, sucursal, tipo, rol, depto, id, correo) {
    $.ajax({
        type: 'POST',
        url: 'wsusuarios.asmx/ActualizarDatos',
        data: '{usuario: "' + usuario + '", nombre : "' + nombre + '",  apellidos : "' + apellidos + '",  password:"' + password + '", empresa: ' + empresa + ', sucursal: ' + sucursal + ', tipo: "' + tipo + '", depto: ' + depto + ', rol: ' + rol + ', id: ' + id + ', correo: "' + correo + '"}',
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
                mostrarUsuarios();
                limpiarForm();
            }

        }
    });
}

//metodo utilizado para insertar un vendedor
function eliminar(id) {
    $.ajax({
        type: 'POST',
        url: 'wsusuarios.asmx/Eliminar',
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
                mostrarUsuarios();
                limpiarForm();
            }

        }
    });
}

//metodo utilizado para mostrar lista de datos 
function mostrarUsuarios() {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsusuarios.asmx/ObtenerDatos",
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
                tds = tds + '<td>' + this.usuario + '</td>';
                tds = tds + '<td>' + this.nombre + '</td>';
                tds = tds + '<td>' + this.empresa + '</td>';
                if (this.idtipo == "A") {
                    tds = tds + '<td>Multisucursal</td>';
                }
                else {
                    tds = tds + '<td>Unisucursal</td>';                
                }
                tds = tds + '<td>' + this.sucursal + '</td>';
                tds = tds + '<td>' + this.rol + '</td>';
                tds = tds + '<td class="center">';  
                tds = tds + "<button class='btn btn-sm btn-outline-info' onclick='Editar(" + this.id + ","+this.idempresa +","+ this.idsucursal + ", " + this.idrol + ", " + this.iddepto + ",\"" + this.nombre + "\",\""+this.apellido+"\",\"" + this.password + "\",\"" + this.usuario + "\",\"" + this.idtipo +  "\",\""+ this.correo +"\")' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' >";
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

function obtenerDeptos() {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsusuarios.asmx/obtenerDeptos",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (msg) {
            tds = '<option value="0" selected>Seleccione un departamento</option>';
            $.each(msg.d, function () {

                tds += '<option value="' + this.id + '" >' + this.departamento + '</option>';
            });

            $('#depto').append(tds);
        }

    });

}


function obtenerSuc(empresa) {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsusuarios.asmx/obtenerSuc",
        data: '{emp : '+empresa+'}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (msg) {
            tds = '<option value="0" selected>Seleccione una sucursal</option>';
            $.each(msg.d, function () {
                tds += '<option value="' + this.id + '" >' + this.sucursal + '</option>';
            });

            $('#sucursal').append(tds);
        }

    });

}

function filtrarSuc(sucursal, empresa) {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsusuarios.asmx/obtenerSuc",
        data: '{emp : ' + empresa + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (msg) {
            tds = '<option value="0">Seleccione una sucursal</option>';
            $.each(msg.d, function () {
                if (sucursal == this.id) {
                    tds += '<option value="' + this.id + '" selected >' + this.sucursal + '</option>';
                }
                else {
                    tds += '<option value="' + this.id + '">' + this.sucursal + '</option>';                
                }
            });

            $('#sucursal').append(tds);
        }

    });
}

function obtenerRoles() {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsroles.asmx/ObtenerDatos",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (msg) {
            tds = '<option value="0" selected>Seleccione un Rol</option>';
            $.each(msg.d, function () {

                tds += '<option value="' + this.id + '" >' + this.nombre + '</option>';
            });

            $('#rol').append(tds);
        }

    });

}

function limpiarForm() {
    $('#id').val("");
    $('#nombre').val("");
    $('#apellidos').val("");
    $('#password').val("");
    $('#confirm').val("");
    $('#emp').val(0);
    $('#sucursal').val(0);
    $('#tipoU').val(0);
    $('#usuario').val("");
    $('#rol').val(0);
    $('#depto').val(0);
    $('#correo').val("");
}