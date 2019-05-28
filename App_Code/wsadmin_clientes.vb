Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class wsadmin_clientes
    Inherits System.Web.Services.WebService

    'accion para obtener las bodegas
    <WebMethod()>
    Public Function ObtenerDatos() As List(Of datos)
        Dim SQL As String = "SELECT c.Id_Clt,c.nit_clt,c.Nom_clt,c.Tel_Clt,c.Dire_Clt,c.id_clasif,c.id_dep,c.id_muni,c.contacto1,c.contacto2,c.contacto3,c.Fax_Clt,c.Descuento_Porc,c.Limite_Credito,c.Dias_Credito,c.Correo_Clt,c.Observ_Clt " &
            "FROM  [CLiente] c " &
            "WHERE c.estado = 1;"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("Id_Clt")
                Elemento.nit = TablaEncabezado.Rows(i).Item("nit_clt")
                Elemento.nombre = TablaEncabezado.Rows(i).Item("Nom_clt")
                Elemento.Telefono = TablaEncabezado.Rows(i).Item("Tel_Clt")
                Elemento.Direccion = TablaEncabezado.Rows(i).Item("Dire_Clt")
                Elemento.id_clasificacion = TablaEncabezado.Rows(i).Item("id_clasif")
                Elemento.id_dep = TablaEncabezado.Rows(i).Item("id_dep")
                Elemento.id_mun = TablaEncabezado.Rows(i).Item("id_muni")
                Elemento.contacto1 = TablaEncabezado.Rows(i).Item("contacto1")
                Elemento.contacto2 = TablaEncabezado.Rows(i).Item("contacto2")
                Elemento.contacto3 = TablaEncabezado.Rows(i).Item("contacto3")
                Elemento.descuento = TablaEncabezado.Rows(i).Item("Descuento_Porc")
                Elemento.limite = TablaEncabezado.Rows(i).Item("Limite_Credito")
                Elemento.dias = TablaEncabezado.Rows(i).Item("Dias_Credito")
                Elemento.correo = TablaEncabezado.Rows(i).Item("Correo_Clt")
                Elemento.observaciones = TablaEncabezado.Rows(i).Item("Observ_Clt")
                result.Add(Elemento)
                ii = ii & 1
            Next
        Next

        Return result

    End Function


    'Metodo para Guardar Los datos
    <WebMethod()>
    Public Function Insertar(ByVal nit_clt As String, ByVal Nom_clt As String, ByVal Tel_Clt As String, ByVal Dire_Clt As String, ByVal id_clasif As Integer, ByVal id_dep As Integer, ByVal id_muni As Integer, ByVal contacto1 As String, ByVal contacto2 As String, ByVal contacto3 As String, ByVal Descuento_Porc As Integer, ByVal Limite_Credito As Double, ByVal Dias_Credito As Integer, ByVal Correo_Clt As String, ByVal Observ_Clt As String) As String
        'consulta sql
        Dim sql As String = "INSERT INTO [dbo].[CLiente](nit_clt,Nom_clt,Tel_Clt,Dire_Clt,id_clasif,id_dep,id_muni,contacto1,contacto2,contacto3,Descuento_Porc,Limite_Credito,Dias_Credito,Correo_Clt,Observ_Clt, estado) " &
            "VALUES ('" & nit_clt & "','" & Nom_clt & "','" & Tel_Clt & "','" & Dire_Clt & "'," & id_clasif & "," & id_dep & "," & id_muni & ",'" & contacto1 & "','" & contacto2 & "','" & contacto3 & "','" & Descuento_Porc & "','" & Limite_Credito & "','" & Dias_Credito & "','" & Correo_Clt & "','" & Observ_Clt & "',1) "


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Insertador Correctamente."
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function


    'Metodo para Guardar Los datos
    <WebMethod()>
    Public Function InsertarRetornaID(ByVal nit_clt As String, ByVal Nom_clt As String, ByVal Tel_Clt As String, ByVal Dire_Clt As String, ByVal id_clasif As Integer, ByVal id_dep As Integer, ByVal id_muni As Integer, ByVal contacto1 As String, ByVal contacto2 As String, ByVal contacto3 As String, ByVal Descuento_Porc As Integer, ByVal Limite_Credito As Double, ByVal Dias_Credito As Integer, ByVal Correo_Clt As String, ByVal Observ_Clt As String) As String
        'consulta sql
        Dim sql As String = "INSERT INTO [dbo].[CLiente](nit_clt,Nom_clt,Tel_Clt,Dire_Clt,id_clasif,id_dep,id_muni,contacto1,contacto2,contacto3,Descuento_Porc,Limite_Credito,Dias_Credito,Correo_Clt,Observ_Clt, estado) " &
            "VALUES ('" & nit_clt & "','" & Nom_clt & "','" & Tel_Clt & "','" & Dire_Clt & "'," & id_clasif & "," & id_dep & "," & id_muni & ",'" & contacto1 & "','" & contacto2 & "','" & contacto3 & "','" & Descuento_Porc & "','" & Limite_Credito & "','" & Dias_Credito & "','" & Correo_Clt & "','" & Observ_Clt & "',1) "


        Dim result As String = ""

        If ExisteNit(nit_clt) Then
            result = "ERROR|ESTE NIT YA ESTA REGISTRADO. "
        Else
            Dim id As Integer = manipular.EjecutaTransaccion_devolverid(sql)
            'ejecuta el query a travez de la clase manipular 
            If Not id = 0 Then
                result = "SUCCESS|Cliente Creado Exitosamente.|" & id
            Else
                result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
            End If
        End If

        Return result
    End Function


    'Metodo para Actualizar Los datos
    <WebMethod()>
    Public Function Actualizar(ByVal nit_clt As String, ByVal Nom_clt As String, ByVal Tel_Clt As String, ByVal Dire_Clt As String, ByVal id_clasif As Integer, ByVal id_dep As Integer, ByVal id_muni As Integer, ByVal contacto1 As String, ByVal contacto2 As String, ByVal contacto3 As String, ByVal Descuento_Porc As Integer, ByVal Limite_Credito As Double, ByVal Dias_Credito As Integer, ByVal Correo_Clt As String, ByVal Observ_Clt As String, ByVal id As Integer) As String
        'consulta sql
        Dim sql As String = "UPDATE [dbo].[CLiente] SET nit_clt='" + nit_clt + "',Nom_clt= '" + Nom_clt + "',Tel_Clt = '" + Tel_Clt + "',Dire_Clt = '" + Dire_Clt + "',id_clasif = " & id_clasif & ",id_dep = " & id_dep & ",id_muni = " & id_muni & ",contacto1 = '" & contacto1 & "',contacto2 = '" & contacto2 & "',contacto3 = '" & contacto3 & "',Descuento_Porc = '" & Descuento_Porc & "',Limite_Credito = '" & Limite_Credito & "',Dias_Credito = '" & Dias_Credito & "',Correo_Clt= '" & Correo_Clt & "',Observ_Clt = '" & Observ_Clt & "' where Id_Clt = " & id

        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Insertador Correctamente."
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function

    <WebMethod()>
    Public Function ExisteNit(ByVal nit As String) As Boolean
        Dim SQL As String = "SELECT count(Id_Clt) as cantidad FROM  [CLiente] where nit_clt = '" & nit & "';"

        Dim result As Boolean = False
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
        Dim cantidad As Integer = 0
        For i = 0 To TablaEncabezado.Rows.Count - 1

            cantidad = TablaEncabezado.Rows(i).Item("cantidad")
        Next


        If cantidad > 0 Then
            result = True
        Else
            result = False
        End If

        Return result

    End Function


    'Metodo para Eliminar Los datos
    <WebMethod()>
    Public Function Inhabilitar(ByVal id As Integer) As String
        'consulta sql
        Dim sql As String = "UPDATE  [CLiente] set estado = 0 where Id_Clt = " & id


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Actualizados Correctamente"
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function

    Public Class datos
        Public id As Integer
        Public nit As String
        Public nombre As String
        Public Telefono As String
        Public Direccion As String
        Public id_clasificacion As Integer
        Public id_dep As Integer
        Public id_mun As Integer
        Public contacto1 As String
        Public contacto2 As String
        Public contacto3 As String
        Public descuento As Integer
        Public limite As Double
        Public dias As Integer
        Public correo As String
        Public observaciones As String
    End Class

End Class