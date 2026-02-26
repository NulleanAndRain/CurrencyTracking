CREATE TABLE IF NOT EXISTS users (
	id UUID PRIMARY KEY,
	"name" varchar(255) NOT NULL,
	"password" varchar(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS currencies (
	id varchar(20) PRIMARY KEY,
	"name" varchar(255) NOT NULL UNIQUE,
	rate numeric NOT NULL
);

CREATE TABLE IF NOT EXISTS user_favorites (
	"user_id" UUID NOT NULL references users(id),
	currency_id varchar(20) NOT NULL references currencies(id),
	PRIMARY KEY ("user_id", currency_id)
);
