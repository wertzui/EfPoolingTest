SELECT ConnectionStatus = CASE WHEN dec.most_recent_sql_handle = 0x0
        THEN 'Unused'
        ELSE 'Used'
        END
    , CASE WHEN des.status = 'Sleeping'
        THEN 'sleeping'
        ELSE 'Not Sleeping'
        END
    , ConnectionCount = COUNT(1)
    , des.login_name
FROM sys.dm_exec_connections dec
    INNER JOIN sys.dm_exec_sessions des
        ON dec.session_id = des.session_id
GROUP BY CASE WHEN des.status = 'Sleeping'
        THEN 'sleeping'
        ELSE 'Not Sleeping'
        END
    , CASE WHEN dec.most_recent_sql_handle = 0x0
        THEN 'Unused'
        ELSE 'Used'
        END
    , des.login_name