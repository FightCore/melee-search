using MeleeSearch.Data.Context;
using MeleeSearch.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeleeSearch.DataLoader;

public static class AliasSeeder
{
    public static async Task SeedAliasesAsync(MeleeSearchDbContext context)
    {
        var commonAliases = new Dictionary<string, string>
        {
            // Character aliases
            { "falcon", "captain falcon" },
            { "c. falcon", "captain falcon" },
            { "cf", "captain falcon" },
            { "puff", "jigglypuff" },
            { "jiggs", "jigglypuff" },
            { "purin", "jigglypuff" },
            { "doc", "dr. mario" },
            { "dr mario", "dr. mario" },
            { "ics", "ice climbers" },
            { "icies", "ice climbers" },
            { "gnw", "mr. game & watch" },
            { "g&w", "mr. game & watch" },
            { "game and watch", "mr. game & watch" },
            { "mr game and watch", "mr. game & watch" },
            { "ylink", "young link" },
            { "yl", "young link" },
            { "dk", "donkey kong" },

            // Move type aliases
            { "cc", "crouch cancel" },
            { "wd", "wavedash" },
            { "wl", "waveland" },
            { "sh", "short hop" },
            { "fh", "full hop" },
            { "shffl", "short hop fast fall l-cancel" },
            { "l-cancel", "lcancel" },
            { "di", "directional influence" },
            { "sdi", "smash directional influence" },
            { "asdi", "automatic smash directional influence" },
            { "tech", "ukemi" },
            { "dair", "down air" },
            { "uair", "up air" },
            { "fair", "forward air" },
            { "bair", "back air" },
            { "nair", "neutral air" },
            { "utilt", "up tilt" },
            { "dtilt", "down tilt" },
            { "ftilt", "forward tilt" },
            { "usmash", "up smash" },
            { "dsmash", "down smash" },
            { "fsmash", "forward smash" },
            { "grab", "command grab" },
            { "upb", "up special" },
            { "up-b", "up special" },
            { "sideb", "side special" },
            { "side-b", "side special" },
            { "downb", "down special" },
            { "down-b", "down special" },
            { "neutralb", "neutral special" },
            { "neutral-b", "neutral special" },

            // Fox-specific moves
            { "shine", "reflector" },
            { "waveshine", "wave shine" },

            // Marth-specific moves
            { "tipper", "tip" },

            // Other common terms
            { "dd", "dash dance" },
            { "pp", "perfect pivot" },
            { "ledgedash", "ledge dash" },
            { "edgeguard", "edge guard" },
            { "edgehog", "edge hog" },
            { "moonwalk", "moon walk" },
        };

        var now = DateTime.UtcNow;
        var addedCount = 0;

        foreach (var (term, replacement) in commonAliases)
        {
            var existingAlias = await context.Aliases
                .FirstOrDefaultAsync(a => a.Term.ToLower() == term.ToLower());

            if (existingAlias == null)
            {
                var alias = new Alias
                {
                    Term = term,
                    Replacement = replacement,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                context.Aliases.Add(alias);
                addedCount++;
            }
        }

        if (addedCount > 0)
        {
            await context.SaveChangesAsync();
            Console.WriteLine($"Added {addedCount} new aliases to the database");
        }
        else
        {
            Console.WriteLine("No new aliases to add");
        }
    }
}
