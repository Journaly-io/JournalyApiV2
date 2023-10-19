using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("JournalEntry")]
public class JournalEntry
{
    [Key]
    [Column("Uuid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Uuid { get; set; }
    
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }
    
    [Column("Owner")]
    public Guid Owner { get; set; }
    
    [Column("Body")]
    public string Body { get; set; }
    
    [Column("Deleted")]
    public bool Deleted { get; set; }
    
    // Navigation properties
    public ICollection<ActivityEntry> ActivityEntries { get; set; }
    public ICollection<EmotionEntry> EmotionEntries { get; set; }
    public ICollection<JournalEntryCategoryValue> JournalEntryCategoryValues { get; set; }
}