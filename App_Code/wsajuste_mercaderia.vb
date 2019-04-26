Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsajuste_mercaderia
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function ObtenerExistencias() As IList(Of productos)

        Dim sql As String = "  SELECT a.id_art, e.Existencia_Deta_Art, a.Des_Art, a.cod_Art  FROM [ERPDEVLYNGT].[dbo].[Existencias] e INNER JOIN [ERPDEVLYNGT].[dbo].[Articulo] a on a.id_art = e.Id_Art  "

        Dim result As List(Of productos) = New List(Of productos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(sql)
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As productos = New productos()
                Elemento.id = TablaEncabezado.Rows(i).Item("id_art")
                Elemento.cantidad = TablaEncabezado.Rows(i).Item("Existencia_Deta_Art")
                Elemento.Descripcion = TablaEncabezado.Rows(i).Item("Des_Art")
                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art").ToString
                result.Add(Elemento)

                ii = ii + 1
            Next
        Next

        Return result
    End Function

    <WebMethod()>
    Public Function RealizarAjuste(ByVal datos_ajuste As List(Of ajuste_mer), ByVal usuario As String) As String

        Dim result As String = ""

        Dim sql As String = "INSERT INTO [dbo].[ENC_AJUSTE]([fecha],[usuario],[id_empresa],[observaciones]) VALUES(GETDATE(),'" & usuario & "',(select id_empresa from USUARIO where USUARIO = '" & usuario & "'),'AJUSTE DE MERCADERIA');"

        Dim id As Integer = manipular.EjecutaTransaccion_devolverid(sql)

        If id = 0 Then
            result = "Error | VERIQUE LA INSERCIÓN DE LA COMPRA"
        Else
            result = "SUCCESS|"
            For Each item As ajuste_mer In datos_ajuste
                Dim cantidad As Integer = ObtenerCantidadProducto(item.id, item.bodega)


                If item.tipo = 2 Then
                    If cantidad >= item.cantidad Then
                        'INSERTA LOS DATOS 
                        Dim sql2 As String = "INSERT INTO [dbo].[DET_AJUSTE]([IdAjuste],[Id_Art],[Id_Bod],[cantidad],[observaciones]) VALUES (" & id & "," & item.id & "," & item.bodega & "," & item.cantidad & ",'" & item.observacion & "');" &
                            " UPDATE [dbo].[Existencias] SET [Existencia_Deta_Art] = " & cantidad - item.cantidad & " WHERE Id_Bod = " & item.bodega & " and Id_Art = " & item.id


                        If manipular.EjecutaTransaccion1(sql2) Then

                            result = result & "AJUSTE GENERADO EXITOSAMENTE EXITOSAMENTE PRODUCTO " & item.descripcion

                        Else
                            result = result & "ERROR VERIQUE EL AJUSTE DEL  PRODUCTO: " & item.descripcion
                        End If


                    Else
                        result = "UN ARTICULO NO SE PUEDE REDUCIR YA QUE NO EXISTE LA SUFICIENTE CANTIDAD DE PRODUCTO ( " & item.codigo & ", CANT EXT " & cantidad & ", CAN RET " & item.cantidad & ")"
                    End If
                Else
                    'INSERTA LOS DATOS 
                    Dim sql2 As String = "INSERT INTO [dbo].[DET_AJUSTE]([IdAjuste],[Id_Art],[Id_Bod],[cantidad],[observaciones]) VALUES (" & id & "," & item.id & "," & item.bodega & "," & item.cantidad & ",'" & item.observacion & "');" &
                        " UPDATE [dbo].[Existencias] SET [Existencia_Deta_Art] = " & cantidad + item.cantidad & " WHERE Id_Bod = " & item.bodega & " and Id_Art = " & item.id


                    If manipular.EjecutaTransaccion1(sql2) Then

                        result = result & "AJUSTE GENERADO EXITOSAMENTE EXITOSAMENTE PRODUCTO " & item.descripcion

                    Else
                        result = result & "ERROR VERIQUE EL AJUSTE DEL  PRODUCTO: " & item.descripcion
                    End If

                End If
            Next
        End If
        Return result
    End Function


    <WebMethod()>
    Public Function ObtenerCantidadProducto(ByVal idart As Integer, ByVal idbodega As Integer) As Integer
        Dim SQL As String = "Select Existencia_Deta_Art as cantidad from ERPDEVLYNGT.dbo.Existencias where Id_Art = " & idart & " and id_bod = " & idbodega

        Dim result As Integer = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("cantidad")
            Next
        Next

        Return result

    End Function


    Public Class productos
        Public id As Integer
        Public descripcion As String
        Public codigo As String
        Public cantidad As Integer
    End Class


    Public Class ajuste_mer
        Public cantidad As Integer
        Public codigo As String
        Public descripcion As String
        Public tipo As Integer
        Public id As Integer
        Public observacion As String
        Public bodega As Integer
        Public bod As String
    End Class

End Class