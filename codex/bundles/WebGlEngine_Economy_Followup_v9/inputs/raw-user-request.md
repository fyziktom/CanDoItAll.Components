# Raw user request

codex už bundle dodělal. oboje je pushnuté. podívej se na to jak to provedl. Pravděpodobně to bude vyžadovat hardening/refaktoring. prozkoumej také jaké jsou hlavní slabiny současného řešení a jak je napravit.
Co ještě potřebujeme k tomu, abychom mohli efektivně zkoumat naše ekonomické simulace? Nebo jsou tam pořád obecné nedostatky, které by nám zanšely naše experimenty šumem (scénář nedopadne ne kvůli špatnému ekonomickému modelu, ale kvůli bugu v simulátoru). Toto musíme mít opravdu dobře ošetřené. Proběhly všechny opravy které jsi minule navrhovala?
Podívej se i na to jestli nám to generických částí neunikají nějaké doménové pojmy. Hlavně kolem těch našich příkladů. Nesmí to být "šité" jen na ně. Toto je potřeba důkladně ověřovat.
Bylo by vhodné přidat třetí příklad který bude trochu odlišný. například ekonomický model s X kategoriemi zboží, možností burzy pro směny a investice bohatých do menších entit. Například pro sledování monopolizace, nebo vznik elit obecně. Je to dost komplexní, ale zároveň odlišná od našich existujících příkladů, takže nám to ukáže nějaké jasné mezery v genericitě našeho simulačního i vizualizačního jádra.
Udělej opět důkladnou analýzu a připrav followup bundle.
