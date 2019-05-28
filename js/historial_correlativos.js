$(function () {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsadmin_correlativos.asmx/ObtenerHistorial",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                tds += '<tr class="odd">';
                tds = tds + '<td>' + this.fecha + '</td>';
                tds = tds + '<td style="text-align: justify">' + this.informacion + '</td>';
                tds = tds + '<td>' + this.usuario + '</td>';
                tds = tds + '</tr>';
            });
            $("#tbod-datos").append(tds);
        }

    });
});