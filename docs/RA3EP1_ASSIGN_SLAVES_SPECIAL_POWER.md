# AssignSlavesTargetObjectSpecialPower

<!-- Reborn: expose the actual inherited EP1 behavior rather than a fixed repair-building XML profile. -->
The behavior is enabled through the normal complete-GameObject build path.
It inherits `SpecialPowerModuleData` and adds no native fields. Author a unique
module `id` and a valid `SpecialPowerTemplate`; defaults come from the official
XSD. IDs and compatible template targets are not pinned to the stock repair
building. Existing inherited scalar fields and RequirementsFilterMPSkirmish
string lists use the shared schema/native proof. Other inherited nested records
remain subject to the existing native ABI and stock metadata grammar support.

`AssignSlavesSpecialPowerContract.Qualify` independently checks the original
SovietCrane and AlliedWarFactory modules against their source XML, native data,
import identities and metadata inheritance. `ConscriptVariableGraphVerifier`
then proves each authored module and verifies the generated metadata against
that grammar. The existing native marshaler/dispatcher is reused.

Private regression cases cover defaults, edited scalar/list/reference values
and two separately named modules in one object, through full stream/BIG
validation with corrupted-native/metadata controls. No live mod source or game
files are changed by the tests. This is offline build evidence, not a gameplay
test of repair-drone behavior. Reborn source admission now passes this module;
the next observed stop is `SovietPortBuilding.xml`'s root `Browser` attribute.
