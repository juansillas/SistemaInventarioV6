using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos.Especificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        //Se necesita una variable de DbContext
        private readonly ApplicationDbContext _db;
        //Propiedad DbSet
        internal DbSet<T> dbSet;

        //Contructor
        public Repositorio(ApplicationDbContext db)
        {
            //Inicializar DbContext y DbSet
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        //IMPLEMENTAR MÉTODOS DE LA INTERFAZ IRepositorio
        public async Task Agregar(T entidad)
        {
            await dbSet.AddAsync(entidad); //es como usar insert into Table
        }

        public async Task<T> Obtener(int id)
        {
            return await dbSet.FindAsync(id); 
        }

   

        public async Task<T> ObtenerPrimero(Expression<Func<T, bool>> filtro = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            //Trabajar con todos los parámetros
            if (filtro != null)
            {
                query = query.Where(filtro); //select * from where
            }
            //incluirPropiedades es una cadena de caracteres
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp);  //Ejemplo "Categoria, Marca"
                }
            }
            //Ordenamiento
           
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> ObtenerTodos(Expression<Func<T, bool>> filtro = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            //Trabajar con todos los parámetros
            if (filtro != null)
            {
                query = query.Where(filtro); //select * from where
            }
            //incluirPropiedades es una cadena de caracteres
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp);  //Ejemplo "Categoria, Marca"
                }
            }
            //Ordenamiento
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        public PagedList<T> ObtenerTodosPaginado(Parametros parametros, Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            //Trabajar con todos los parámetros
            if (filtro != null)
            {
                query = query.Where(filtro); //select * from where
            }
            //incluirPropiedades es una cadena de caracteres
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp);  //Ejemplo "Categoria, Marca"
                }
            }
            //Ordenamiento
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return PagedList<T>.ToPagedList(query, parametros.PageNumber, parametros.PageSize);
        }

        public void Remover(T entidad)
        {
            dbSet.Remove(entidad);
        }

        public void RemoverRango(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }
    }
}
