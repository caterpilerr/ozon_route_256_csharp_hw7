create table payment_lifecycle
(
	id int8 default nextval('payment_lifecycle_id_seq'::regclass) not null,
	entity_id uuid not null,
	version int4 not null,
	type int4 not null,
	data text not null
);

create sequence payment_lifecycle_id_seq
    start 1
    increment 1;

create unique index payment_lifecycle_id_uindex
	on payment_lifecycle (id);

create unique index payment_lifecycle_pk
	on payment_lifecycle (id);

alter table payment_lifecycle
	add constraint payment_lifecycle_pk
		primary key (id);

