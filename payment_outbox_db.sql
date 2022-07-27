create table payment_outbox 
(
    id uuid primary key ,
    payment_id uuid not null,
    type int not null,
    data text not null,
    try_count int not null
);