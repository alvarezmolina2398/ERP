$(function () {
    getfechaCierre();
    //formato para date picker
    $('input[name="datepicker"]').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        minYear: 1901,
        locale: {
            format: 'YYYY-MM-DD'
        }
    }, function (start, end, label) { });


    //metodo utilizado para obtener el reporte de orden de compra
    $('#btnConsultar').click(function () {
        $.ajax({
            type: 'POST',
            url: 'wscortecaja.asmx/resumen',
            data: '{fechaIni: "' + $('#inicio').val() + '",fechaFin: "' + $('#fin').val() + '", suc: 1}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {

            },
            success: function (msg) {
                var tds = "";
                $.each(msg.d, function () {
                    $('#efec').text(this.efectivo);
                    $('#vis').text(this.visa);
                    $('#cre').text(this.credomatic);
                    $('#che').text(this.cheque);
                    $('#emp').text(this.empresasAs);
                    $('#des').text(this.descuentoE);
                    $('#subt').text(this.subtotal);
                    $('#nc').text(this.notaCredito);
                    $('#tot').text(this.total);
                    $('#fechaC').val(this.fechaCierre);
                    $('#fc').val(this.fechaCierre);
                });
                consultarGeneral();
            }

        });
    });

    $('#btnGenerar').click(function () {
        $.ajax({
            type: 'POST',
            url: 'wscortecaja.asmx/generarCierre',
            data: '{fCierre : "' + $('#fechaC').val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                $('#btnGenerar').attr('disabled', true);
                $('#btnGenerar').text('Cargando...');
            },
            success: function (msg) {
                $('#btnGenerar').text('Geerar Cierre');
                $('#btnGenerar').removeAttr('disabled');
                $.toast({
                    heading: 'EXITO!',
                    text: 'El cierre se realizo exitosamente',
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'success',
                    stack: false
                })
                getfechaCierre();
            }

        });
    });



});

function consultarGeneral() {
    $.ajax({
      type: 'POST',
        url: 'wscortecaja.asmx/Consultar',
        data: '{fechaIni: "' + $('#inicio').val() + '",fechaFin: "' + $('#fin').val() + '", suc: 1}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $('#tbod').empty();
        },
        success: function (msg) {
            var tds = "";
            $.each(msg.d, function () {
                tds += '<tr class="odd">';
                tds = tds + '<td>' + this.firma + '</td>';
                tds = tds + '<td>' + this.nit + '</td>';
                tds = tds + '<td>' + this.cliente + '</td>';
                tds = tds + '<td style="text-align: right;">' + this.total + '</td>';
                tds = tds + '<td style="text-align: right;">' + this.sinIva + '</td>';
                tds = tds + '<td>' + this.usuario + '</td>';
                tds = tds + '<td>' + this.tipo + '</td>';
                tds = tds + "<td><button  class='btn btn-outline-dark' onclick='getDetalle(" + this.efectivo +","+this.visa +","+this.credomatic+","+this.cheque+",\"" + this.empleado + "\",\""+this.empresa+"\",\"" +this.orden+"\",\""+this.cliente+"\",\""+this.firma+ "\")'><i class='material-icons'>assignment</i></button></td>";
                tds = tds + '</tr>';
            });

            $("#tbod").append(tds);
        }
    });
      

}


function getDetalle(efectivo, visa, credomatic, cheque, empleado, empresa, orden, cliente, firma) {
    $('#or').text(orden);
    $('#ef').text(efectivo);
    $('#vi').text(visa);
    $('#cr').text(credomatic);
    $('#ch').text(cheque);
    $('#em').text(empleado);
    $('#ea').text(empresa); 
    $('#clt').text(cliente);
    $('#com').text(firma);
    $('#MdNuevo').modal('toggle');
}

function getfechaCierre() {
    $.ajax({
        type: 'POST',
        url: 'wscortecaja.asmx/getFechaCierre',
        data: '{}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {

        },
        success: function (msg) {
            $('#fechaC').val(msg.d);
            $('#fc').text(msg.d);

        }

    });
}