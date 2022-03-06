import React, { useEffect, useState } from "react";
import Select from 'react-select'
import { HubConnectionBuilder } from '@microsoft/signalr';
import { useTable } from 'react-table'

import './Home.css';

function Table({ columns, data }) {
    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable({
        columns,
        data,
    })
    // Render the UI for your table
    return (
        <>
            <table {...getTableProps()} className="styled-table">
                <thead>
                    {headerGroups.map(headerGroup => (
                        <tr {...headerGroup.getHeaderGroupProps()}>
                            {headerGroup.headers.map(column => (
                                <th {...column.getHeaderProps()}>{column.render('Header')}</th>
                            ))}
                        </tr>
                    ))}
                </thead>

                <tbody {...getTableBodyProps()}>
                    {rows.map((row, i) => {
                        prepareRow(row);
                        return (
                            <tr {...row.getRowProps()}>
                                {row.cells.map(cell => {
                                    return <td {...cell.getCellProps()}>{cell.render('Cell')}</td>;
                                })}
                            </tr>
                        );
                    })}
                </tbody>
            </table>
        </>
    )
}

export const Home = () => {
    const [isLoading, setIsLoading] = useState(true);
    const [users, setUsers] = useState([]);
    const [storeId, setStoreId] = useState('');
    const [connection, setConnection] = useState(null);



    const columns = React.useMemo(() => [
        {
            Header: 'Clients dans le magasin (temps réel)',
            columns: [
                {
                    Header: 'Client',
                    accessor: 'userId',
                },
                {
                    Header: 'Entrée',
                    accessor: 'inTimestamp',
                },
            ],
        }
    ], []);

    const parseTimeStamp = (timestamp) => {
        const date = new Date(timestamp);
        return date.getHours() + ':' + date.getMinutes() + ':' + date.getSeconds();
    };

    useEffect(() => {
        if (storeId) {
            setConnection(new HubConnectionBuilder()
                .withUrl(`http://localhost:8080/storehub?storeId=${storeId.value}`)
                .build());
        }
    }, [storeId]);

    useEffect(() => {
        if (connection) {
            connection.start().then(() => {
                setIsLoading(false);
                connection.on("ReceivedMessage", (data) => {
                    setUsers(data.map((d) => {
                        return {
                            userId: d.userId,
                            inTimestamp: parseTimeStamp(d.inTimestamp),
                        }
                    }));
                });
                connection.invoke("GetBaseList").then((data) => {
                    setUsers(data.map((d) => {
                        return {
                            userId: d.userId,
                            inTimestamp: parseTimeStamp(d.inTimestamp),
                        }
                    }));
                });
            });
            return () => {
                setUsers([]);
                connection.stop();
            };
        }
    }, [connection]);

    const options = [
        { label: "Fnac", value: "Fnac" },
        { label: "Monoprix", value: "Monoprix" },
        { label: "Carrefour", value: "Carrefour" },
        { label: "Darty", value: "Darty" }
    ]

    return (
        <div className="w-100">
            <h1>Selectionner un magasin</h1>
            <Select options={options} value={storeId} onChange={setStoreId} />
            {isLoading ? (
                <p>Connection...</p>
            ) : (
                <>
                    <h2>Magasin : {storeId.value} </h2>
                    <p>Nombre de Client: {users.length}</p>
                    <Table columns={columns} data={users} />
                </>
            )}
        </div>
    );
};