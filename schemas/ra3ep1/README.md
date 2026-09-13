# Red Alert 3 Uprising schemas

This directory contains the official Uprising XSD set copied from the local
`RA3 Uprising Reborn/Official/Schemas` reference repository. The files are kept
separate from `schemas/ra3` because Uprising uses manifest version 7 and a
different type-system fingerprint (`0x5454A8E9`).

Do not point the production compiler at this schema set until its v7 manifest
writer and Uprising type metadata/serializer are enabled. Schema validation
alone does not make version-5 Kane's Wrath output Uprising-compatible.
