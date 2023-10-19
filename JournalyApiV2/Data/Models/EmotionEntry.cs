using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("EmotionEntry")]
public class EmotionEntry
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("EmotionUuid")]
    [ForeignKey("Emotion")]
    public Guid EmotionUuid { get; set; }
    
    [Column("JournalEntryUuid")]
    [ForeignKey("JournalEntry")]
    public Guid JournalEntryUuid { get; set; }
    
    // Navigation properties
    public Emotion Emotion { get; set; }
    public JournalEntry JournalEntry { get; set; }
}