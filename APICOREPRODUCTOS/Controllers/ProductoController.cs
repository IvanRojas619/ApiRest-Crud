using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APICOREPRODUCTOS.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Cors;

namespace APICOREPRODUCTOS.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly string cadenaSql;

        public ProductoController(IConfiguration config)
        {
            cadenaSql = config.GetConnectionString("CadenaSql"); // Almaceno mi cadena de conexion que esta en el archivo appsettings

        }
        //Obtencion de lista de productos
        [HttpGet]
        [Route("Lista")]

        public IActionResult Lista()
        {
            List<Producto> lista = new List<Producto>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSql))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_lista_productos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Producto()
                            {
                                //NOMBRE MARCA CATEGORIA PRECIO
                                IdProducto = Convert.ToInt32(rd["IdProducto"]),
                                CodigoBarra = rd["CodigoBarra"].ToString(),
                                Nombre = rd["Nombre"].ToString(),
                                Marca = rd["Categoria"].ToString(),
                                Categoria = rd["Categoria"].ToString(),
                                Precio = Convert.ToDecimal(rd["Precio"])

                            }); ;
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Correcto!", response = lista });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message, response = lista });

            }


        }
        //Obtencion de producto por id
        [HttpGet]
        [Route("Obtener/{idproducto:int}")]

        public IActionResult Obtener(int idproducto)
        {
            Producto oproducto = new Producto();
            try
            {
                using (var conexion = new SqlConnection(cadenaSql))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_obtener_producto", conexion);
                    cmd.Parameters.AddWithValue("idproducto", idproducto);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                oproducto.IdProducto = Convert.ToInt32(rd["IdProducto"]);
                                oproducto.CodigoBarra = rd["CodigoBarra"].ToString();
                                oproducto.Nombre = rd["Nombre"].ToString();
                                oproducto.Marca = rd["Marca"].ToString();
                                oproducto.Categoria = rd["Categoria"].ToString();
                                oproducto.Precio = Convert.ToDecimal(rd["Precio"]);
                            }
                        }
                        else
                        {
                            return BadRequest("No encontrado");
                        }


                    }

                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Ok! registro encontrado", response = oproducto });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message, response = oproducto });
            }

        }
        //Guardado de registro nuevo

        [HttpPost]
        [Route("Guardar")]

        public IActionResult Guardar([FromBody] Producto producto)
        {
            if(producto == null)
            {
                return BadRequest("No se recibió nungun producto en el body!");
            }
            try
            {
                using (var conexion = new SqlConnection(cadenaSql))
                {
                    conexion.Open(); ;
                    var cmd = new SqlCommand("sp_guardar_producto", conexion);
                    cmd.Parameters.AddWithValue("codigoBarra", producto.CodigoBarra);
                    cmd.Parameters.AddWithValue("nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("marca", producto.Marca);
                    cmd.Parameters.AddWithValue("categoria", producto.Categoria);
                    cmd.Parameters.AddWithValue("precio", producto.Precio);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok! registro exitoso" });
            }catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Fallo en el registro" });

            }

        }

        //Editar producto

        [HttpPut]
        [Route("Editar")]

        public IActionResult Editar([FromBody] Producto producto)
        {
            if (producto == null)
            {
                return BadRequest("No se recibió nungun producto en el body!");
            }
            try
            {
                using (var conexion = new SqlConnection(cadenaSql))
                {
                    conexion.Open(); ;
                    var cmd = new SqlCommand("sp_editar_producto", conexion);
                    cmd.Parameters.AddWithValue("idProducto", producto.IdProducto == 0 ? DBNull.Value : producto.IdProducto);
                    cmd.Parameters.AddWithValue("codigoBarra", producto.CodigoBarra is null ? DBNull.Value : producto.CodigoBarra);
                    cmd.Parameters.AddWithValue("nombre", producto.Nombre is null ? DBNull.Value : producto.Nombre);
                    cmd.Parameters.AddWithValue("marca", producto.Marca is null ? DBNull.Value : producto.Marca);
                    cmd.Parameters.AddWithValue("categoria", producto.Categoria is null ? DBNull.Value : producto.Categoria);
                    cmd.Parameters.AddWithValue("precio", producto.Precio==0 ? DBNull.Value : producto.Precio);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok! registro Actualizado" });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Fallo al actualizar" });

            }

        }

        [HttpDelete]

        [Route("Eliminar/{idProducto:int}")]

        public IActionResult Eliminar(int idProducto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSql))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_eliminar_producto", conexion);
                    cmd.Parameters.AddWithValue("idProducto", idProducto);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Ok! registro eliminado con éxito" });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }


        }


    }
}
