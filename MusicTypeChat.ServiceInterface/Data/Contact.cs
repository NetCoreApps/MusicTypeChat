using System;
using MusicTypeChat.ServiceModel.Types;

namespace MusicTypeChat.ServiceInterface.Data;

/// <summary>
/// Example of using separate DataModel and DTO
/// </summary>
public class Contact
{
    public int Id { get; set; }
    public int UserAuthId { get; set; }
    public Title Title { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public FilmGenre? FavoriteGenre { get; set; }
    public int Age { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
