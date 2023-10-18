using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("Activity")]
public class Activity
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
    
    [Column("Deleted")]
    public bool Deleted { get; set; }
    
    // Navigation propetries
    public IconType IconType { get; set; }
}