namespace FeedsApi.Data.Entities
{
    public class ImageFeed : Feed
    {
        //Alternatively image can be stored in blob storage and url can be stored in db
        public byte[] ImageData { get; set; }
    }
}
