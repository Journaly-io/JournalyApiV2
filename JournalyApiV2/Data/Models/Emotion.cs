using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("Emotion")]
public class Emotion
{
    [Key]
    [Column("Uuid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Uuid { get; set; }
    
    [Column("Name")]
    public string Name { get; set; }
    
    [Column("Owner")]
    public Guid Owner { get; set; }
    
    [Column("Icon")]
    public string Icon { get; set; }
    
    [Column("IconTypeId")]
    public short IconTypeId { get; set; }
    
    [Column("Order")]
    public short Order { get; set; }
    
    [Column("CategoryUuid")]
    [ForeignKey("EmotionCategory")] // It will try to do EmotionCategoryUuid if we dont specify
    public Guid CategoryUuid { get; set; }
    
    [Column("Deleted")]
    public bool Deleted { get; set; } = false;
    
    // Navigation properties
    public EmotionCategory EmotionCategory { get; set; }
    public IconType IconType { get; set; }
    public ICollection<EmotionEntry> EmotionEntries { get; set; }
}