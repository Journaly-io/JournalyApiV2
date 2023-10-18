using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("IconType")]
public class IconType
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short Id { get; set; }
    
    [Column("Name")]
    public string Name { get; set; }
    
    // Navigation properties
    public ICollection<Emotion> Emotions { get; }
    public ICollection<Activity> Activities { get; }
}