import { useEffect, useState } from "react";
import { PaginatedTable, MsgViewButton, LoadingSpinner } from "components";

function MsgTable() {
  const columns = [
    {
      Header: "MsgTable",
      columns: [
        {
          Header: "Sequence Number",
          accessor: "sequenceNumber",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "Timestamp",
          accessor: "timestamp",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "Stream",
          accessor: "stream",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "Subject",
          accessor: "subject",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "Data",
          accessor: "data",
          appendChildren: "true",
          rowBound: "true",
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
    const interval = setInterval(getAllMessages, 1000);
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
        <PaginatedTable columns={columns} data={allMessages}>
          <MsgViewButton content={""} />
        </PaginatedTable>
      )}
    </>
  );
}

export { MsgTable };
