CREATE SCHEMA sec;
GO

CREATE or alter FUNCTION sec.fn_sec_predicate(@candidate AS varchar(100))
    RETURNS TABLE
        WITH SCHEMABINDING
        AS
        RETURN SELECT 1 as can_execute
               WHERE session_context(N'owner') = @candidate
                  OR session_context(N'owner') = 'admin';
GO

DROP SECURITY POLICY HistoryFilter

CREATE SECURITY POLICY HistoryFilter
    ADD FILTER PREDICATE sec.fn_sec_predicate(Owner)
        ON dbo.History
    WITH (STATE = ON);

select *
from sec.fn_sec_predicate(N'jon')

ALTER SECURITY POLICY HistoryFilter
    WITH (state = on)

EXEC sys.sp_set_session_context @key= N'owner', @value= N'jon'

select *
from sec.fn_sec_predicate(N'jon')

SELECT *
from History