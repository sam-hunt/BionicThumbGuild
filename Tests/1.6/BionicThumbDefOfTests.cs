using System.Reflection;
using RimWorld;
using Verse;
using Xunit;

namespace BionicThumbGuild.Tests
{
    // BionicThumbDefOf is populated by RimWorld's [DefOf] startup scan, which
    // matches each static field's *name* against a loaded ThingDef's defName —
    // there's no live DefDatabase headlessly, so that population can't be
    // exercised here. What can be pinned without one: the class carries
    // [DefOf] at all, and the field both patches reference (BTG_BionicThumb)
    // exists with the right type. A rename here would leave the field null at
    // runtime with no compile error, silently breaking both trader-stock
    // patches and the surgery recipe.
    public class BionicThumbDefOfTests
    {
        [Fact]
        public void BionicThumbDefOf_IsMarkedWithDefOfAttribute()
        {
            Assert.NotNull(typeof(BionicThumbDefOf).GetCustomAttribute<DefOf>());
        }

        [Fact]
        public void BionicThumbDefOf_DeclaresBionicThumbFieldAsThingDef()
        {
            FieldInfo field = typeof(BionicThumbDefOf).GetField(
                "BTG_BionicThumb", BindingFlags.Public | BindingFlags.Static);

            Assert.NotNull(field);
            Assert.Equal(typeof(ThingDef), field.FieldType);
        }
    }
}
