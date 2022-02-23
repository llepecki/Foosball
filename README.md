# Foosbal Score API

aka "Track your foosball scores the hard way"

---

## Usage

`dotnet run --database-connection "[your connection string]"`

The database must exists but the application will create tables, indexes, etc. itself in case they don't exist.

By default the application listens on `http://localhost:5002` (`https` is disabled), Swagger `http://localhost:5002/swagger`.

## Why no test?

It really hurts my heart but at some point I needed to say stop due to the time constraints (which I exceeded anyway).

## Why EventFlow?

It has a lot of blocks that one can use out of the box. Not having to write them by myself let me focus more on the domain.

## Why the read model is so flat?

I sacrificed fanciness for convenience and efficiency.