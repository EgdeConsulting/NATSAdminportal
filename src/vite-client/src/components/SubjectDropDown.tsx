import { Select } from "@chakra-ui/react";
import { LegacyRef, useEffect, useState } from "react";

function SubjectDropDown(props: {
  subjectInputRef: LegacyRef<HTMLSelectElement>;
  checkInputs: () => void;
}) {
  const [subjects, setSubjects] = useState<[]>([]);

  function getSubjects() {
    fetch("/api/subjectNames")
      .then((res) => res.json())
      .then((data) => {
        setSubjects(data); //Should consider removing stars from subjects?
      });
  }

  useEffect(() => {
    getSubjects();
  }, [subjects.length != 0]);

  return (
    <Select
      ref={props.subjectInputRef}
      placeholder="Select a subject"
      onChange={props.checkInputs}
    >
      {subjects.map((subject: any, index: number) => {
        return <option key={index}>{subject["name"]}</option>;
      })}
    </Select>
  );
}

export { SubjectDropDown };
