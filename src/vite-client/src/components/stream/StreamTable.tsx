import { useEffect, useState } from "react";
import { PaginatedTable, StreamViewButton } from "components";

function StreamTable() {
  const columns = [
    {
      Header: "StreamTable",
      columns: [
        {
          Header: "Name",
          accessor: "name",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "No. Subjects",
          accessor: "subjectCount",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "No. Consumers",
          accessor: "consumerCount",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "No. Messages",
          accessor: "messageCount",
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

  const [streams, setStreams] = useState<any[]>([]);

  useEffect(() => {
    getStreams();
  }, [streams.length != 0]);

  function getStreams() {
    fetch("/api/allStreams")
      .then((res) => res.json())
      .then((data) => {
        setStreams(data);
      });
  }

  return (
    <PaginatedTable columns={columns} data={streams}>
      <StreamViewButton content={""} />
    </PaginatedTable>
  );
}
export { StreamTable };
