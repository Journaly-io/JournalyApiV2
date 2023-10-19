using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("JournalEntryCategoryValue")]
public class JournalEntryCategoryValue
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("JournalEntryUuid")]
    [ForeignKey("JournalEntry")]
    public Guid JournalEntryUuid { get; set; }
    
    [Column("CategoryUuid")]
    [ForeignKey("EmotionCategory")]
    public Guid CategoryUuid { get; set; }
    
    [Column("Value")]
    public double Value { get; set; }
    
    // Navigation properties
    public JournalEntry JournalEntry { get; set; }
    public EmotionCategory EmotionCategory { get; set; }
}