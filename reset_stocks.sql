DELETE FROM PricePeriod WHERE 1 = 1;
DELETE FROM StockEvents WHERE 1 = 1;

update Stocks
set UnitPrice = 17.50
where Symbol = 'SUS';

update Stocks
set UnitPrice = 550
where Symbol = 'FRK';

update Stocks
set UnitPrice = 7.5
where Symbol = 'NRM';

update Stocks
set UnitPrice = 5.5
where Symbol = 'SNG';

update Stocks
set UnitPrice = 101
where Symbol = 'CCO';