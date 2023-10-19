using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalyApiV2.Data.Models;

[Table("ActivityEntry")]
public class ActivityEntry
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("ActivityUuid")]
    [ForeignKey("Activity")]
    public Guid ActivityUuid { get; set; }
    
    [Column("JournalEntryUuid")]
    [ForeignKey("JournalEntry")]
    public Guid JournalEntryUuid { get; set; }
    
    // Navigation properties
    public Activity Activity { get; set; }
    public JournalEntry JournalEntry { get; set; }
}