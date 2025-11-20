using AutoMapper;
using VentasApiPrueba.Models.DTOs;
using VentasApiPrueba.Models.Entities;

namespace VentasApiPrueba.Mappings;


/// Perfil de AutoMapper para mapear entre Entities y DTOs

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Producto
        CreateMap<Producto, ProductoDto>()
            .ForMember(dest => dest.CategoriaNombre, 
                opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : null));
        CreateMap<CreateProductoDto, Producto>()
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));
        CreateMap<UpdateProductoDto, Producto>()
            .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(src => DateTime.Now));

        // Cliente
        CreateMap<Cliente, ClienteDto>();
        CreateMap<CreateClienteDto, Cliente>()
            .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));
        CreateMap<UpdateClienteDto, Cliente>();

        // Venta
        CreateMap<Venta, VentaDto>()
            .ForMember(dest => dest.ClienteNombre, 
                opt => opt.MapFrom(src => src.Cliente.Nombre))
            .ForMember(dest => dest.UsuarioNombre,
                opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Username : null))
            .ForMember(dest => dest.UsuarioRol,
                opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Rol : null))
            .ForMember(dest => dest.Pagado,
                opt => opt.MapFrom(src => src.Pagado))
            .ForMember(dest => dest.Saldo,
                opt => opt.MapFrom(src => src.Total - src.Pagado))
            .ForMember(dest => dest.DetallesVenta, 
                opt => opt.MapFrom(src => src.DetallesVenta));
        CreateMap<CreateVentaDto, Venta>()
            .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => "Completada"))
            .ForMember(dest => dest.DetallesVenta, opt => opt.Ignore());

        // DetalleVenta
        CreateMap<DetalleVenta, DetalleVentaDto>()
            .ForMember(dest => dest.ProductoNombre, 
                opt => opt.MapFrom(src => src.Producto.Nombre));
        CreateMap<CreateDetalleVentaDto, DetalleVenta>();

        // Categoria
        CreateMap<Categoria, CategoriaDto>();
        CreateMap<CreateCategoriaDto, Categoria>()
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));
        CreateMap<UpdateCategoriaDto, Categoria>();

        // Usuario
        CreateMap<Usuario, UsuarioDto>();
        CreateMap<RegisterDto, Usuario>()
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Se hashea manualmente
    }
}
