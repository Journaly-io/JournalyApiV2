using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("EmotionCategory")]
public class EmotionCategory
{
    [Key] 
    [Column("Uuid")] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Uuid { get; set; }

    [Column("Name")] public string Name { get; set; }

    [Column("AllowMultiple")] public bool AllowMultiple { get; set; } = false;

    [Column("Default")] public bool Default { get; set; } = false;

    [Column("Order")] public short Order { get; set; }

    [Column("Deleted")]
    public bool Deleted { get; set; } = false;
    
    [Column("Owner")]
    public Guid Owner { get; set; }
    
    // Navigation properties
    public ICollection<Emotion> Emotions { get; }
}