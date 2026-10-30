namespace DulcesDuendesApp.Models.Dtos
{
    public static class ImagenProductoMapper
    {
        public static ImagenProductoResponse ToResponse(this ImagenProducto i)
        {
            return new ImagenProductoResponse { Id = i.Id, Url = i.Url };
        }
    }
}
