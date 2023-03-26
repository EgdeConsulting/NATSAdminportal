import { useEffect, useState } from "react";
import {
  PaginatedTable,
  MsgViewButton,
  LoadingSpinner,
  SelectColumnFilter,
} from "components";

function MsgTable() {
  const columns = [
    {
      Header: "MsgTable",
      columns: [
        {
          Header: "Sequence Number",
          accessor: "sequenceNumber",
          disableFilters: true,
        },
        {
          Header: "Timestamp",
          accessor: "timestamp",
          Filter: SelectColumnFilter,
          filter: "includes",
        },
        {
          Header: "Stream",
          accessor: "stream",
          Filter: SelectColumnFilter,
          filter: "includes",
        },
        {
          Header: "Subject",
          accessor: "subject",
          Filter: SelectColumnFilter,
          filter: "includes",
        },
        {
          Header: "Data",
          accessor: "data",
          disableFilters: true,
          Cell: (props: { row: any }) => {
            return <MsgViewButton content={props.row.values} />;
          },
        },
      ],
    },
  ];

  const [allMessages, setAllMessages] = useState<any[]>([]);
  const [isIntervalRunning, setIsIntervalRunning] = useState(false);
  const [loading, setLoading] = useState(true);

  function getAllMessages() {
    fetch("/api/allMessages").then((res: any) => {
      if (res.ok) {
        res.json().then((data: any) => {
          setAllMessages(data);
          setLoading(false);
        });
      } else {
        alert(
          "An error occurred while fetching all messages: " + res.statusText
        );
      }
    });
  }

  useEffect(() => {
    setIsIntervalRunning(true);
    const interval = setInterval(getAllMessages, 3000);
    return () => {
      clearInterval(interval);
      setIsIntervalRunning(false);
    };
  }, [!isIntervalRunning]);

  return (
    <>
      {loading ? (
        <LoadingSpinner />
      ) : (
        <PaginatedTable columns={columns} data={allMessages} />
      )}
    </>
  );
}

export { MsgTable };
