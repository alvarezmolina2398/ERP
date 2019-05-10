$(function () {
    cargarRegiones();

    //accion para cargar las sucursales al cambiar de region
    $('#region').change(function () {
        $('#sucursal').html('<option value="0">Seleccione Una Sucursal</option>');
        $('#bodega').html('<option value="0">Seleccione Una Sucursal</option>');
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


    });


    $('#btn-generar').click(function () {
        mostrarDatos($('#region').val(), $('#sucursal').val(), $('#bodega').val());
    })

    //accion para cargar las sucursales al cambiar de region
    $('#sucursal').change(function () {
        $('#bodega').html('<option value="0">Seleccione Una Bodega</option>');

        //consume el ws para obtener los datos
        $.ajax({
            url: 'wsadmin_bodegas.asmx/ObtenerDatosIDSucursal',
            data: '{sucursal: ' + $(this).val() + '}',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function (msg) {
                $.each(msg.d, function () {
                    $('#bodega').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                });
            }
        });



    });

});


function cargarRegiones() {
    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_regiones.asmx/ObtenerDatos',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#region').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}

//metodo utilizado para mostrar lista de datos 
function mostrarDatos(region, sucursal, bodega) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wstomainventarios.asmx/ObtenerListadoProductos',
        data: '{bodega : '+bodega+', sucursal: '+sucursal+', region : '+region+'}',
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
                tds = "<tr class='odd'><td>" + i + "</td><td>" + this.codigo + "</td><td>" + this.producto + "</td><td>" + this.Bodega + "</td><td>" + this.costo + "</td><td>" + this.existencia + "</td><td>" + this.tomadeinventarios +"</td><td>"+this.diferencia+"</td></tr>"
                i++;

                $("#tbod-datos").append(tds);
            });

            $('#tab-datos').dataTable();
            $('[data-toggle="popover"]').popover();
        }
    });

};

