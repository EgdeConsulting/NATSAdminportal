import { useEffect, useState } from "react";
import {
  PaginatedTable,
  MsgViewButton,
  LoadingSpinner,
  SelectColumnFilter,
  IMsg,
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
          disableFilters: true,
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

  const [allMessages, setAllMessages] = useState<IMsg[]>([]);
  const [isIntervalRunning, setIsIntervalRunning] = useState(false);
  const [loading, setLoading] = useState(true);

  function getAllMessages() {
    fetch("/api/allMessages").then((res) => {
      if (res.ok) {
        res.json().then((data: IMsg[]) => {
          setAllMessages(data);
          setLoading(false);
        });

      } else if (res.status == 429) {

        console.log("API was too busy to handle request.");
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
